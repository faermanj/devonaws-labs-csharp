// Copyright 2013 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"). You may not 
// use this file except in compliance with the License. A copy of the License 
// is located at
// 
// 	http://aws.amazon.com/apache2.0/
// 
// or in the "LICENSE" file accompanying this file. This file is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either 
// express or implied. See the License for the specific language governing 
// permissions and limitations under the License.

using System;
using System.Configuration;
using System.IO;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AwsLabs
{
    internal class Program
    {
        // Use the region endpoint that is nearest to you, or one that is recommended by your instructor.
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        #region Non-Student Code
        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        private const string LAB_USER_NAME = "LabAppUser";

        public static void Main(string[] args)
        {
            LabVariables labVariables = null;
            var program = new Program();
            try
            {
                // Start the "prep" mode operations to make sure that the resources are all in the expected state.
                Console.WriteLine("Starting up in \"prep\" mode.");
                labVariables = program.PrepMode_Run();

                Console.WriteLine("\nPrep complete. Transitioning to \"app\" mode.");
                program.AppMode_Run(labVariables);
            }
            catch (Exception ex)
            {
                LabUtility.DumpError(ex);
            }
            finally
            {
                try
                {
                    
                    if (labVariables != null)
                    {
                        Console.Write("\nLab run completed. Cleaning up buckets.");
                        
                        AWSCredentials credentials =
                            new BasicAWSCredentials(ConfigurationManager.AppSettings["prepModeAWSAccessKey"],
                                ConfigurationManager.AppSettings["prepModeAWSSecretKey"]);

                        var s3Client = new AmazonS3Client(credentials, RegionEndpoint);

                        OptionalLabCode.RemoveLabBuckets(s3Client, labVariables.BucketNames);
                        Console.WriteLine(" Done.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nAttempt to clean up buckets failed. {0}", ex.Message);
                }

                Console.WriteLine("\nPress <enter> to end.");
                Console.ReadLine();
            }
        }


        /// <summary>
        ///     Prepare the resources used in the lab.
        /// </summary>
        /// <returns>Details of the created resources.</returns>
        public LabVariables PrepMode_Run()
        {
            AWSCredentials credentials =
                new BasicAWSCredentials(ConfigurationManager.AppSettings["prepModeAWSAccessKey"],
                    ConfigurationManager.AppSettings["prepModeAWSSecretKey"]);


            var labVariables = new LabVariables();
            using (var iamClient = new AmazonIdentityManagementServiceClient(credentials, RegionEndpoint))
            {
                string trustRelationship = File.OpenText("TrustRelationship.txt").ReadToEnd();
                string developmentPolicyText = File.OpenText("development_role.txt").ReadToEnd();
                string productionPolicyText = File.OpenText("production_role.txt").ReadToEnd();

                // Clean up environment by removing the roles if they exist. 
                OptionalLabCode.PrepMode_RemoveRoles(iamClient, "development_role", "production_role");

                // Trust relationships for roles (the way we're using them) require the ARN of the user.
                string userArn = LabCode.PrepMode_GetUserArn(iamClient, LAB_USER_NAME);
                Console.WriteLine("ARN for {0} is {1}", LAB_USER_NAME, userArn);
                trustRelationship = trustRelationship.Replace("{userArn}", userArn);
                Console.WriteLine("Trust relationship policy:\n{0}", trustRelationship);

                // Create the roles and store the role ARNs
                labVariables.DevelopmentRoleArn = LabCode.PrepMode_CreateRole(iamClient, "development_role",
                    developmentPolicyText, trustRelationship);
                labVariables.ProductionRoleArn = LabCode.PrepMode_CreateRole(iamClient, "production_role",
                    productionPolicyText, trustRelationship);

                Console.WriteLine("Created development policy role: {0}", labVariables.DevelopmentRoleArn);
                Console.WriteLine("Created production policy role: {0}", labVariables.ProductionRoleArn);

                // Create the bucket names
                string identifier = Guid.NewGuid().ToString().Substring(0, 8);
                labVariables.BucketNames.Add("dev" + identifier);
                labVariables.BucketNames.Add("prod" + identifier);

                // Create the buckets
                using (var s3Client = new AmazonS3Client(credentials, RegionEndpoint))
                {
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        OptionalLabCode.PrepMode_CreateBucket(s3Client, bucketName);
                        Console.WriteLine("Created bucket: {0}", bucketName);
                    }
                }
            }

            return labVariables;
        }

        /// <summary>
        ///     Perform the AppMode operations by assuming the dev and prod roles, and checking for permissions.
        /// </summary>
        /// <param name="labVariables">The data.</param>
        public void AppMode_Run(LabVariables labVariables)
        {
            var credentials = new BasicAWSCredentials(
                ConfigurationManager.AppSettings["appModeAWSAccessKey"],
                ConfigurationManager.AppSettings["appModeAWSSecretKey"]);

            Credentials devCredentials = null, prodCredentials = null;

            using (var stsClient = new AmazonSecurityTokenServiceClient(credentials, RegionEndpoint))
            {
                Console.WriteLine("Assuming developer role to retrieve developer session credentials.");
                devCredentials = LabCode.AppMode_AssumeRole(stsClient, labVariables.DevelopmentRoleArn, "dev_session");
                if (devCredentials == null)
                {
                    Console.WriteLine("No developer credentials returned. AccessDenied.");
                    return;
                }


                Console.WriteLine("\nAssuming production role to retrieve production session credentials.");

                prodCredentials = LabCode.AppMode_AssumeRole(stsClient, labVariables.ProductionRoleArn, "prod_session");
                if (prodCredentials == null)
                {
                    Console.WriteLine("No production credentials returned. AccessDenied.");
                    return;
                }
            }

            using (var devS3Client = LabCode.AppMode_CreateS3Client(devCredentials, RegionEndpoint))
            {
                using (var prodS3Client = LabCode.AppMode_CreateS3Client(prodCredentials, RegionEndpoint))
                {
                    Console.WriteLine("\nTesting Developer Session...");
                    var devSession = new SessionAWSCredentials(
                        devCredentials.AccessKeyId,
                        devCredentials.SecretAccessKey,
                        devCredentials.SessionToken);

                    Console.WriteLine("  IAM: {0}",
                        OptionalLabCode.AppMode_TestIamAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SQS: {0}",
                        OptionalLabCode.AppMode_TestSqsAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SNS: {0}",
                        OptionalLabCode.AppMode_TestSnsAccess(RegionEndpoint, devSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  S3:");
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        TestS3Client(devS3Client, bucketName);
                    }

                    Console.WriteLine("\nTesting Production Session...");
                    var prodSession = new SessionAWSCredentials(
                        prodCredentials.AccessKeyId,
                        prodCredentials.SecretAccessKey,
                        prodCredentials.SessionToken);

                    Console.WriteLine("  IAM: {0}",
                        OptionalLabCode.AppMode_TestIamAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SQS: {0}",
                        OptionalLabCode.AppMode_TestSqsAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  SNS: {0}",
                        OptionalLabCode.AppMode_TestSnsAccess(RegionEndpoint, prodSession)
                            ? "Accessible."
                            : "Inaccessible.");
                    Console.WriteLine("  S3:");
                    foreach (string bucketName in labVariables.BucketNames)
                    {
                        TestS3Client(prodS3Client, bucketName);
                    }
                }
            }
        }

        /// <summary>
        ///     Test access to the specified S3 bucket by adding an object to it.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The bucket name</param>
        public void TestS3Client(AmazonS3Client s3Client, string bucketName)
        {
            const string fileName = "test-image.png";

            Console.Write("    Uploading to bucket {0}. ", bucketName);
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                FilePath = new FileInfo(fileName).FullName,
                Key = fileName
            };

            try
            {
                s3Client.PutObject(putObjectRequest);
                Console.WriteLine("Succeeded.");
            }
            catch (AmazonS3Exception ase)
            {
                Console.WriteLine("Failed. {0}.", ase.ErrorCode);
            }
            catch
            {
                Console.WriteLine("Failed.");
            }
        }

        #endregion
    }
}

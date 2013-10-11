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

using System.Collections.Generic;
using Amazon;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     Find and return the ARN for the specified user.
        ///     Hint: Use the GetUser() method of the client object. The ARN for the user is in the response.
        /// </summary>
        /// <param name="iamClient">The IAM client object.</param>
        /// <param name="userName">The name of the user to find.</param>
        /// <returns>The ARN of the specified user.</returns>
        public override string PrepMode_GetUserArn(AmazonIdentityManagementServiceClient iamClient, string userName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.PrepMode_GetUserArn(iamClient, userName);
        }

        /// <summary>
        ///     Create the specified role using the specified policy and trust relationship text. Return the role ARN.
        /// </summary>
        /// <param name="iamClient">Tha IAM client object.</param>
        /// <param name="roleName">The name of the role to create.</param>
        /// <param name="policyText">The policy to attach to the role.</param>
        /// <param name="trustRelationshipText">The policy defining who can assume the role.</param>
        /// <returns>The ARN for the newly created role.</returns>
        public override string PrepMode_CreateRole(AmazonIdentityManagementServiceClient iamClient, string roleName,
            string policyText,
            string trustRelationshipText)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.PrepMode_CreateRole(iamClient, roleName, policyText, trustRelationshipText);
        }

        /// <summary>
        ///     Assume the specified role.
        ///     Hint: Use the AssumeRole() method of the client object.
        ///     Optional: You may see an eventual consistency issue here. The AssumeRole permissions may not
        ///     have propagated through the system yet which could prevent us from assuming the role. Check for
        ///     an AmazonServiceException with an ErrorCode of "AccessDenied" and retry the assume role operation
        ///     after a short wait (with exponential back-off on retries). If you decide to stop retrying,
        ///     return null.
        /// </summary>
        /// <param name="stsClient">The STS client object.</param>
        /// <param name="roleArn">The ARN of the role to assume.</param>
        /// <param name="roleSessionName">The name to use as the role session name.</param>
        /// <returns>The role credentials, or null if there was a problem.</returns>
        public override Credentials AppMode_AssumeRole(AmazonSecurityTokenServiceClient stsClient, string roleArn,
            string roleSessionName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.AppMode_AssumeRole(stsClient, roleArn, roleSessionName);
        }

        /// <summary>
        ///     Create session/temporary credentials using the provided credentials (previously returned from the AssumeRole
        ///     method), and use the session credentials to create an S3 client object.
        /// </summary>
        /// <param name="credentials">The credentials to use for creating session credentials.</param>
        /// <param name="regionEndpoint">The region endpoint to use for the client.</param>
        /// <returns>The S3 client object.</returns>
        public override AmazonS3Client AppMode_CreateS3Client(Credentials credentials, RegionEndpoint regionEndpoint)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.AppMode_CreateS3Client(credentials, regionEndpoint);
        }

        #region Optional Tasks

        /// <summary>
        ///     Test access to the IAM service using the provided credentials by requesting a listing of the IAM users.
        ///     You are free to test in any way you like. Submit any sort of request and watch for an exception.
        /// </summary>
        /// <param name="regionEndpoint">The region endpoint to use for the client connection.</param>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>True, if the service is accessible. False, if the credentials are rejected.</returns>
        public override bool AppMode_TestIamAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.AppMode_TestIamAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     Test access to the SNS service using the provided credentials by requesting a listing of the SNS topics.
        ///     You are free to test in any way you like. Submit any sort of request and watch for an exception.
        /// </summary>
        /// <param name="regionEndpoint">The region endpoint to use for the client connection.</param>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>True, if the service is accessible. False, if the credentials are rejected.</returns>
        public override bool AppMode_TestSnsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.AppMode_TestSnsAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     Test access to the SQS service using the provided credentials by requesting a listing of the SQS queues.
        ///     You are free to test in any way you like. Submit any sort of request and watch for an exception.
        /// </summary>
        /// <param name="regionEndpoint">The region endpoint to use for the client connection.</param>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>True, if the service is accessible. False, if the credentials are rejected.</returns>
        public override bool AppMode_TestSqsAccess(RegionEndpoint regionEndpoint, SessionAWSCredentials credentials)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.AppMode_TestSqsAccess(regionEndpoint, credentials);
        }

        /// <summary>
        ///     Create a bucket that will be used later in the lab. This is housekeeping code that is used to prepare the
        ///     environment for the lab exercise.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket to create.</param>
        public override void PrepMode_CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.PrepMode_CreateBucket(s3Client, bucketName);
        }

        /// <summary>
        ///     Remove any roles that match the names of the roles we'll be creating. This will be called by the lab controller code
        ///     to clean up resources that might conflict with proper lab execution.
        /// </summary>
        /// <param name="iamClient">The IAM client object.</param>
        /// <param name="roles">The list of role names.</param>
        public override void PrepMode_RemoveRoles(AmazonIdentityManagementServiceClient iamClient, params string[] roles)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.PrepMode_RemoveRoles(iamClient, roles);
        }

        /// <summary>
        ///     Cleanup/delete the buckets that were created by the lab.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketNames">The buckets to remove.</param>
        public override void RemoveLabBuckets(AmazonS3Client s3Client, List<string> bucketNames)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.RemoveLabBuckets(s3Client, bucketNames);
        }

        #endregion
    }
}

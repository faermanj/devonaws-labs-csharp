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
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsLabs
{
    internal class SolutionCode : ILabCode, IOptionalLabCode
    {
        public virtual void CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            // Create the request
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            // Create the bucket
            s3Client.PutBucket(putBucketRequest);
        }

        public virtual void PutObject(AmazonS3Client s3Client, string bucketName, string sourceFile, string objectKey)
        {
            // Create the request
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                FilePath = sourceFile
            };

            // Upload the object
            s3Client.PutObject(putObjectRequest);
        }

        public virtual void ListObjects(AmazonS3Client s3Client, string bucketName)
        {
            // Create the request
            var listObjectsRequest = new ListObjectsRequest
            {
                BucketName = bucketName
            };

            // Submit the request
            ListObjectsResponse listObjectsResponse = s3Client.ListObjects(listObjectsRequest);

            // Display the results
            foreach (S3Object objectSummary in listObjectsResponse.S3Objects)
            {
                Console.WriteLine("{0} (size: {1})", objectSummary.Key, objectSummary.Size);
            }
        }

        public virtual void MakeObjectPublic(AmazonS3Client s3Client, string bucketName, string key)
        {
            // Create the request
            var putAclRequest = new PutACLRequest {
                BucketName = bucketName,
                Key = key,
                CannedACL = S3CannedACL.PublicRead
            };

            // Submit the request
            s3Client.PutACL(putAclRequest);
        }

        public virtual string GeneratePreSignedUrl(AmazonS3Client s3Client, string bucketName, string key)
        {
            // Create the request
            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.Now.AddHours(1.0)
            };

            // Submit the request
            return s3Client.GetPreSignedURL(getPreSignedUrlRequest);
        }

        public virtual void DeleteBucket(AmazonS3Client s3Client, string bucketName)
        {
            // First, try to delete the bucket. 
            var deleteBucketRequest = new DeleteBucketRequest
            {
                BucketName = bucketName
            };

            try
            {
                s3Client.DeleteBucket(deleteBucketRequest);
                // If we get here, no error was generated so we'll assume the bucket was deleted and return.
                return;
            }
            catch (AmazonS3Exception ex)
            {
                if (!ex.ErrorCode.Equals("BucketNotEmpty"))
                {
                    // We got an unanticipated error. Just rethrow.
                    throw;
                }
            }

            // If we got here, then our bucket isn't empty so we need to delete the items in it first.

            DeleteObjectsRequest deleteObjectsRequest = new DeleteObjectsRequest {BucketName = bucketName};

            foreach (S3Object obj in s3Client.ListObjects(new ListObjectsRequest {BucketName = bucketName}).S3Objects)
            {
                // Add keys for the objects to the delete request
                deleteObjectsRequest.AddKey(obj.Key, null);
            }

            // Submit the request
            s3Client.DeleteObjects(deleteObjectsRequest);

            // The bucket is empty now, so delete the bucket.
            s3Client.DeleteBucket(deleteBucketRequest);
        }
    }
}

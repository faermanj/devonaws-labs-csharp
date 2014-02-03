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
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     Use the provided S3 client object to create the specified bucket.
        ///     Hint: Use the PutBucket() method of the client object.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket to create.</param>
        /// <remarks>The purpose of this task is to gain experience working with S3 programmatically.</remarks>
        public override void CreateBucket(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.CreateBucket(s3Client, bucketName);
        }

        /// <summary>
        ///     Upload the provided item to the specified bucket.
        ///     Hint: Use the PutObject() method of the client object.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the target bucket.</param>
        /// <param name="sourceFile">The name of the file to upload.</param>
        /// <param name="objectKey">The key to assign to the new S3 object.</param>
        /// <remarks>The purpose of this task is to gain experience working with S3 programmatically.</remarks>
        public override void PutObject(AmazonS3Client s3Client, string bucketName, string sourceFile, string objectKey)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.PutObject(s3Client, bucketName, sourceFile, objectKey);
        }

        /// <summary>
        ///     List the contents of the specified bucket by writing the object key and item size to the console.
        ///     Hint: Use the ListObjects() method of the client object.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket containing the objects to list.</param>
        /// <remarks>The purpose of this task is to gain experience working with S3 programmatically.</remarks>
        public override void ListObjects(AmazonS3Client s3Client, string bucketName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.ListObjects(s3Client, bucketName);
        }

        /// <summary>
        ///     Change the ACL for the specified object to make it publicly readable.
        ///     Hint: Call the SetACL() method of the client object. Use the S3CannedACL enumeration to set the ACL for the object
        ///     to the canned ACL "PublicRead."
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket containing the object.</param>
        /// <param name="key">The key used to identify the object.</param>
        /// <remarks>The purpose of this task is to gain experience working with S3 programmatically.</remarks>
        public override void MakeObjectPublic(AmazonS3Client s3Client, string bucketName, string key)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.MakeObjectPublic(s3Client, bucketName, key);
        }

        /// <summary>
        ///     Create and return a pre-signed URL for the specified item. Set the URL to expire one hour from the moment it was
        ///     generated.
        ///     Hint: Use the GetPreSignedURL() method of the client object.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket containing the object.</param>
        /// <param name="key">The key used to identify the object.</param>
        /// <remarks>The purpose of this task is to gain experience working with S3 programmatically.</remarks>
        /// <returns>The pre-signed URL for the object.</returns>
        public override string GeneratePreSignedUrl(AmazonS3Client s3Client, string bucketName, string key)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GeneratePreSignedUrl(s3Client, bucketName, key);
        }

        #region Optional Tasks

        /// <summary>
        ///     Delete the specified bucket. You will use the DeleteBucket() method of the client object to delete the bucket,
        ///     but first you will need to delete the bucket contents. To delete the contents, you will need to list the objects
        ///     and delete them individually (DeleteObject() method) or as a batch (DeleteObjects() method).
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket to delete.</param>
        /// <remarks>
        ///     The purpose of this task is to gain experience writing applications that remove unused AWS resources
        ///     in an automated manner.
        /// </remarks>
        public override void DeleteBucket(AmazonS3Client s3Client, string bucketName)
        {
            base.DeleteBucket(s3Client, bucketName);
        }

        #endregion
    }
}

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
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     Use the provided S3 client to generate a pre-signed URL for the item identified by the
        ///     specified bucket and key. Set the link to expire in 1 minute.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="key">The key of the object to provide a link for.</param>
        /// <param name="bucket">The bucket containing the object.</param>
        /// <returns>A pre-signed URL for the object.</returns>
        public override string GetUrlForItem(AmazonS3Client s3Client, string key, string bucket)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GetUrlForItem(s3Client, key, bucket);
        }

        /// <summary>
        ///     Return a collection of items from DynamoDB containing the details for the images to display on the page.
        ///     The name of the table containing the items is identified by the value of SESSIONTABLE. Filter the results based
        ///     on the key prefix defined in PARAM3. You should identify the items using the scan operation. The items collection
        ///     is in the result object.
        /// </summary>
        /// <param name="dynamoDbClient">The DynamoDB client object.</param>
        /// <remarks>
        ///     The purpose of this task is to practice using configuration information provided to the application at
        ///     runtime to shape a scan operation against DynamoDB.
        /// </remarks>
        /// <returns>The collection of matching items.</returns>
        public override List<Dictionary<string, AttributeValue>> GetImageItems(AmazonDynamoDBClient dynamoDbClient)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GetImageItems(dynamoDbClient);
        }

        /// <summary>
        ///     Construct and return an S3 client object that applies the region constraint identified in the
        ///     REGION setting.
        /// </summary>
        /// <remarks>
        ///     The purpose of this task is to practice using configuration information provided to the application at
        ///     runtime to identify which region to target.
        /// </remarks>
        /// <returns>The S3 client object.</returns>
        public override AmazonS3Client CreateS3Client(AWSCredentials credentials)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.CreateS3Client(credentials);
        }

        /// <summary>
        ///     Construct and return a DynamoDB client object that applies the region constraint identified in the
        ///     REGION setting.
        /// </summary>
        /// <remarks>
        ///     The purpose of this task is to practice using configuration information provided to the application at
        ///     runtime to identify which region to target.
        /// </remarks>
        /// <returns>The DynamoDB client object.</returns>
        public override AmazonDynamoDBClient CreateDynamoDbClient(AWSCredentials credentials)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.CreateDynamoDbClient(credentials);
        }

        /// <summary>
        ///     This method is used to convert the items in the DynamoDB items collection to elements that can be rendered
        ///     on a web page. To complete the task:
        ///     (1) Loop through the items in the collection and extract the "Key" and "Bucket"
        ///     attribute values.
        ///     (2) Use the key and bucket values to generate a pre-signed URL for each object represented. To generate
        ///     the URL, call your implementation of the GetUrlForItem() method and capture the return value.
        ///     (3) For each item, call the _Default.AddImageToPage() method, passing in the key, bucket, and URL values
        ///     as method parameters.
        /// </summary>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="items">The items collection to parse.</param>
        /// <remarks>
        ///     The purpose of this task is to practice retrieving item attributes from a collection of items
        ///     provided to us in a DynamoDB result object.
        /// </remarks>
        public override void AddItemsToPage(AmazonS3Client s3Client, List<Dictionary<string, AttributeValue>> items)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.AddItemsToPage(s3Client, items);
        }

        #region Optional Tasks

        /// <summary>
        ///     Inspect the DynamoDB table and determine if it contains an item matching the specified
        ///     hash key.
        /// </summary>
        /// <param name="dynamoDbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to search.</param>
        /// <param name="key">The hash key for the item to locate.</param>
        /// <remarks>
        ///     The purpose of this task is to identify the proper process for identifying the existence
        ///     of an item in DynamoDB and implementing the process.
        /// </remarks>
        /// <returns>True if the item exists, false if it doesn't.</returns>
        public override bool IsImageInDynamo(AmazonDynamoDBClient dynamoDbClient, string tableName, string key)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.IsImageInDynamo(dynamoDbClient, tableName, key);
        }

        /// <summary>
        ///     Request the table description for the specified table and return it to the caller.
        ///     Hint: Use the DescribeTable operation.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <remarks>
        ///     The purpose of this task is to gain experience inspecting the structure of DynamoDB tables.
        /// </remarks>
        /// <returns>The table description object. Null if the table wasn't found.</returns>
        public override TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GetTableDescription(ddbClient, tableName);
        }

        /// <summary>
        ///     Validate the schema described by the tableDescription parameter. We expect the table to have
        ///     the following characteristics:
        ///     Schema - At least two attributes, "Key" and "Bucket" both of string types.
        ///     Hash Key - Single attribute named "Key" of type string.
        ///     Range Key - Single attribute named "Bucket" of type string.
        /// </summary>
        /// <param name="tableDescription">The table definition.</param>
        /// <remarks>
        ///     The purpose of this task is to gain experience working with complex data structures that are
        ///     a regular part of interacting with DynamoDB.
        /// </remarks>
        /// <returns>True if the schema matches what we expect, false if the schema was invalid or an exception was thrown.</returns>
        public override bool ValidateSchema(TableDescription tableDescription)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.ValidateSchema(tableDescription);
        }

        /// <summary>
        ///     Return the table status string that is associated with the specified table. The table status is a property
        ///     of the TableDescription object.
        ///     Hint: Call your GetTableDescription() method to get the table description.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to inspect.</param>
        /// <remarks>
        ///     The purpose of this exercise is to gain practice inspecting the status of a DynamoDB table. Looking at the table
        ///     status can give you clues as to why other table operations are failing or let you know when the table reaches the
        ///     state you expect it to be in.
        /// </remarks>
        /// <returns>The table status string. "NOTFOUND" if the table doesn't exist or can't be located.</returns>
        public override string GetTableStatus(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GetTableStatus(ddbClient, tableName);
        }

        /// <summary>
        ///     Pause execution on this thread until the table status matches the provided status string or until a timeout
        ///     is reached. If the value in the timeout parameter is reached (timeout is less than DateTime.Now), then
        ///     throw a TimeoutException.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to inspect.</param>
        /// <param name="status">The desired table status.</param>
        /// <param name="timeout">The time to stop waiting. If null, wait indefinitely.</param>
        /// <remarks>
        ///     The purpose of this exercise is to gain experience using the table status for a practical purpose (waiting until
        ///     the table reaches an expected state).
        /// </remarks>
        public override void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
            DateTime? timeout = null)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.WaitForStatus(ddbClient, tableName, status, timeout);
        }

        /// <summary>
        ///     Delete the specified table. This method will be called if the lab controller code determines that the
        ///     existing table is invalid for the lab.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to delete.</param>
        /// <remarks>
        ///     The purpose of this exercise is to gain experience cleaning up unneeded or improperly provisioned resources.
        /// </remarks>
        public override void DeleteTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.DeleteTable(ddbClient, tableName);
        }

        /// <summary>
        ///     Upload the specified image to S3 and add a reference to the image to DynamoDB. The DynamoDB item representing the
        ///     image
        ///     should contain two attributes:
        ///     Key - The key to the object in S3
        ///     Bucket - The bucket that the object is located in
        ///     Do not set any permissions for the object in S3; keep the restrictive defaults.
        ///     This method will be called if the lab controller code determines that the images used in the lab aren't
        ///     in place or referenced properly in DynamoDB. It will be executed at least once.
        /// </summary>
        /// <param name="dynamoDbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to put the items in.</param>
        /// <param name="s3Client">The S3 client object.</param>
        /// <param name="bucketName">The name of the bucket to use for the objects in S3.</param>
        /// <param name="imageKey">The key representing the object in S3.</param>
        /// <param name="filePath">The full path of the image file to upload.</param>
        /// <remarks>
        ///     The purpose of this task is to gain experience with the process of preparing AWS resources for use by your
        ///     application.
        /// </remarks>
        public override void AddImage(AmazonDynamoDBClient dynamoDbClient, string tableName, AmazonS3Client s3Client,
            string bucketName,
            string imageKey, string filePath)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.AddImage(dynamoDbClient, tableName, s3Client, bucketName, imageKey, filePath);
        }

        /// <summary>
        ///     Create the table that is used in this lab. Don't return from this method until the table state
        ///     is "ACTIVE". Hint: Call the WaitForStatus() method that you implemented earlier in order to wait.
        ///     Build the table to match these parameters:
        ///     Attributes - "Key" a string, and "Bucket" also a string
        ///     Hash Key Attribute - "Key"
        ///     Range Key Attribute - "Bucket"
        ///     Provisioned Capacity - 5 Reads/5 Writes
        ///     This method will be called by the lab controller code at least once in order to prepare the lab. It will
        ///     also be called if the lab controller code determines that the table needs to be rebuilt (ex. the schema
        ///     doesn't match our expectations.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to create.</param>
        public override void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.BuildTable(ddbClient, tableName);
        }

        #endregion Optional Tasks
    }
}

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
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace AwsLabs
{
    internal class SolutionCode : IOptionalLabCode, ILabCode
    {
        
        virtual public string GetUrlForItem(AmazonS3Client s3Client, string key, string bucket)
        {
            var getPreSignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.Now + TimeSpan.FromMinutes(2)

            };
            return s3Client.GetPreSignedURL(getPreSignedUrlRequest); ;
        }

        virtual public List<Dictionary<string, AttributeValue>> GetImageItems(AmazonDynamoDBClient dynamoDbClient)
        {
            try
            {
                string tableName = ConfigurationManager.AppSettings["SESSIONTABLE"];
                string keyPrefix = ConfigurationManager.AppSettings["PARAM3"];

                var scanRequest = new ScanRequest
                {
                    Select = "ALL_ATTRIBUTES",
                    TableName = tableName
                };

                // If the filter criteria is empty, then don't filter it.
                if (!String.IsNullOrEmpty(keyPrefix))
                {
                    scanRequest.ScanFilter = new Dictionary<string, Condition>
                    {
                        {
                            "Key",
                            new Condition
                            {
                                ComparisonOperator = "BEGINS_WITH",
                                AttributeValueList = new List<AttributeValue>
                                {
                                    new AttributeValue {S = keyPrefix}
                                }
                            }
                        }
                    };
                }

                return dynamoDbClient.Scan(scanRequest).Items;

            }
            catch (Exception ex)
            {
                _Default.LogMessageToPage("GetImageItems Error: {0}", ex.Message);
                return null;
            }
        }


        virtual public AmazonS3Client CreateS3Client(AWSCredentials credentials)
        {
            return new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["REGION"]));
        }


        virtual public AmazonDynamoDBClient CreateDynamoDbClient(AWSCredentials credentials)
        {
            
            return new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName(ConfigurationManager.AppSettings["REGION"]));
        }

        virtual public void AddItemsToPage(AmazonS3Client s3Client, List<Dictionary<string, AttributeValue>> items)
        {
            foreach (var item in items)
            {
                AttributeValue key, bucket;
                if (item.TryGetValue("Key", out key) && item.TryGetValue("Bucket", out bucket))
                {
                    string itemUrl = GetUrlForItem(s3Client, key.S, bucket.S);
                    _Default.AddImageToPage(itemUrl, bucket.S, key.S);
                }
            }
        }

        virtual public bool IsImageInDynamo(AmazonDynamoDBClient dynamoDbClient, string tableName, string key)
        {
            try
            {
                var queryRequest = new QueryRequest
                {
                    TableName = tableName,
                    KeyConditions = new Dictionary<string, Condition>
                    {
                        {
                            "Key",
                            new Condition
                            {
                                ComparisonOperator = "EQ",
                                AttributeValueList = new List<AttributeValue>
                                {
                                    new AttributeValue {S = key}
                                }
                            }
                        }
                    },
                    ConsistentRead = true,
                };

                QueryResponse queryResponse = dynamoDbClient.Query(queryRequest);
                return queryResponse.Count > 0;
            }
            catch (Exception ex)
            {
                _Default.LogMessageToPage("IsImageInDynamo Error: {0}", ex.Message);
                return false;
            }
        }

        virtual public bool ValidateSchema(TableDescription tableDescription)
        {
            if (tableDescription == null)
            {
                _Default.LogMessageToPage("Null table description passed to validation method.");
                return false;
            }

            if (!tableDescription.TableStatus.Equals("ACTIVE"))
            {
                _Default.LogMessageToPage("Table is not active.");
                return false;
            }

            if (tableDescription.AttributeDefinitions == null || tableDescription.KeySchema == null)
            {
                _Default.LogMessageToPage("Schema doesn't match.");
                return false;
            }
            foreach (AttributeDefinition attributeDefinition in tableDescription.AttributeDefinitions)
            {
                // Confirm that the relevant attributes are both strings.
                switch (attributeDefinition.AttributeName)
                {
                    case "Key":
                    case "Bucket":
                        if (!attributeDefinition.AttributeType.Equals("S"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            _Default.LogMessageToPage("{0} attribute is wrong type in attribute definition.", attributeDefinition.AttributeName);
                            return false;
                        }
                        break;
                }
            }
            // If we've gotten here, the attributes are good. Now check the key schema.
            if (tableDescription.KeySchema.Count != 2)
            {
                _Default.LogMessageToPage("Wrong number of elements in the key schema.");
                return false;
            }
            foreach (KeySchemaElement keySchemaElement in tableDescription.KeySchema)
            {
                switch (keySchemaElement.AttributeName)
                {
                    case "Key":
                        if (!keySchemaElement.KeyType.Equals("HASH"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            _Default.LogMessageToPage("Key attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    case "Bucket":
                        if (!keySchemaElement.KeyType.Equals("RANGE"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            _Default.LogMessageToPage("Bucket attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    default:
                        _Default.LogMessageToPage(String.Format("Unexpected attribute ({0}) in the key schema.", keySchemaElement.AttributeName));
                        return false;
                }

            }
            _Default.LogMessageToPage("Table schema is as expected.");
            // We've passed our checks.
            return true;
        }

        virtual public TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
        {
            try
            {
                DescribeTableResponse describeTableResponse = ddbClient.DescribeTable(
                    new DescribeTableRequest
                    {
                        TableName = tableName
                    });

                return describeTableResponse.Table;
            }
            catch (AmazonServiceException ase)
            {
                // If the table isn't found, there's no problem. 
                // If the error is something else, rethrow the exception to bubble it up to the caller.
                if (!ase.ErrorCode.Equals("ResourceNotFoundException"))
                {
                    throw;
                }
                return null;
            }
        }

        virtual public void DeleteTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            switch (GetTableStatus(ddbClient, tableName))
            {
                case "ACTIVE":
                    _Default.LogMessageToPage("Deleting pre-existing table.");
                    var deleteTableRequest = new DeleteTableRequest { TableName = tableName };
                    ddbClient.DeleteTable(deleteTableRequest);
                    WaitForStatus(ddbClient, tableName, "NOTFOUND");

                    _Default.LogMessageToPage("Table deletion confirmed.");
                    break;
                case "NOTFOUND":
                    _Default.LogMessageToPage("Skipped deletion operation. Table not found.");
                    break;
                default:
                    _Default.LogMessageToPage("Skipped deletion operation. Table not in correct state.");
                    break;
            }
        }

        virtual public void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
            DateTime? timeout = null)
        {
            while (!GetTableStatus(ddbClient, tableName).Equals(status))
            {
                if (timeout != null && timeout < DateTime.Now)
                {
                    // The timeout is specified, and it's passed, so throw an exception to let the caller know we timed out.
                    throw new TimeoutException(String.Format("The table failed to reach the target state [{0}].", status));
                }
                // Either timeout is null, or it's specified and hasn't passed yet, so sleep.
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        virtual public string GetTableStatus(AmazonDynamoDBClient ddbClient, string tableName)
        {
            TableDescription tableDescription = GetTableDescription(ddbClient, tableName);
            if (tableDescription != null)
            {
                return tableDescription.TableStatus;
            }
            return "NOTFOUND";
        }

        virtual public void AddImage(AmazonDynamoDBClient dynamoDbClient, string tableName, AmazonS3Client s3Client, string bucketName, string imageKey, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {

                    // Create the upload request
                    var putObjectRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = imageKey,
                        FilePath = filePath
                    };

                    // Upload the object
                    s3Client.PutObject(putObjectRequest);

                    // Create the put item request to submit to DynamoDB
                    var putItemRequest = new PutItemRequest
                    {
                        TableName = tableName,
                        Item = new Dictionary<string, AttributeValue>
                        {
                            {"Key", new AttributeValue {S = imageKey}},
                            {"Bucket", new AttributeValue {S = bucketName}}
                        }
                    };

                    dynamoDbClient.PutItem(putItemRequest);
                    _Default.LogMessageToPage("Added imageKey: {0}", imageKey);
                }
                else
                {
                    _Default.LogMessageToPage("Skipped imageKey: {0}", imageKey);
                }
            }
            catch (Exception ex)
            {
                _Default.LogMessageToPage("AddImage Error: {0}", ex.Message);
            }
        }


        virtual public void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            _Default.LogMessageToPage("Creating table.");
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition {AttributeName = "Key", AttributeType = "S"},
                    new AttributeDefinition {AttributeName = "Bucket", AttributeType = "S"}
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement {AttributeName = "Key", KeyType = "HASH"},
                    new KeySchemaElement {AttributeName = "Bucket", KeyType = "RANGE"}
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            ddbClient.CreateTable(request);
            // Pause until the table is active (timeout after 2 mins)
            WaitForStatus(ddbClient, tableName, "ACTIVE", DateTime.Now + TimeSpan.FromMinutes(2));
            _Default.LogMessageToPage("Table created and active.");
        }

    }
}

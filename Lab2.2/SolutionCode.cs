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
using System.Threading;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace AwsLabs
{
    internal class SolutionCode : IOptionalLabCode, ILabCode
    {
        public virtual void CreateAccountItem(AmazonDynamoDBClient ddbClient, string tableName, Account account)
        {
            // Create the request
            var putItemRequest = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Company", new AttributeValue {S = account.Company}},
                    {"Email", new AttributeValue {S = account.Email}}
                }
            };

            // Only add attributes if the coresponding property in the account object is not empty.
            if (!String.IsNullOrEmpty(account.First))
            {
                putItemRequest.Item.Add("First", new AttributeValue {S = account.First});
            }
            if (!String.IsNullOrEmpty(account.Last))
            {
                putItemRequest.Item.Add("Last", new AttributeValue {S = account.Last});
            }
            if (!String.IsNullOrEmpty(account.Age))
            {
                putItemRequest.Item.Add("Age", new AttributeValue {N = account.Age});
            }

            // Submit the request
            ddbClient.PutItem(putItemRequest);
        }

        public virtual QueryResponse LookupByHashKey(AmazonDynamoDBClient ddbClient, string tableName, string company)
        {
            // Build request

            var queryRequest = new QueryRequest
            {
                TableName = tableName,
                KeyConditions = new Dictionary<string, Condition>
                {
                    {
                        "Company",
                        new Condition
                        {
                            ComparisonOperator = "EQ",
                            AttributeValueList = new List<AttributeValue>
                            {
                                new AttributeValue {S = company}
                            }
                        }
                    }
                },
                ConsistentRead = true,
            };

            // Submit request and return the response
            return ddbClient.Query(queryRequest);
        }

        public virtual void UpdateIfMatch(AmazonDynamoDBClient ddbClient, string tableName, string email, string company,
            string firstNameTarget, string firstNameMatch)
        {
            // Build the request
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"Company", new AttributeValue {S = company}},
                    {"Email", new AttributeValue {S = email}}
                },
                Expected = new Dictionary<string, ExpectedAttributeValue>
                {
                    {"First", new ExpectedAttributeValue {Value = new AttributeValue {S = firstNameMatch}}}
                },
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    {
                        "First",
                        new AttributeValueUpdate {Action = "PUT", Value = new AttributeValue {S = firstNameTarget}}
                    }
                }
            };

            // Submit request 
            ddbClient.UpdateItem(updateItemRequest);
        }

        public virtual void DeleteTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            switch (GetTableStatus(ddbClient, tableName))
            {
                case "ACTIVE":
                    Console.WriteLine("Deleting pre-existing table.");
                    var deleteTableRequest = new DeleteTableRequest {TableName = tableName};
                    ddbClient.DeleteTable(deleteTableRequest);
                    WaitForStatus(ddbClient, tableName, "NOTFOUND");

                    Console.WriteLine("Table deletion confirmed.");
                    break;
                case "NOTFOUND":
                    Console.WriteLine("Skipped deletion operation. Table not found.");
                    break;
                default:
                    Console.WriteLine("Skipped deletion operation. Table not in correct state.");
                    break;
            }
        }


        public virtual void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            Console.WriteLine("Creating table.");
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition {AttributeName = "Company", AttributeType = "S"},
                    new AttributeDefinition {AttributeName = "Email", AttributeType = "S"}
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement {AttributeName = "Company", KeyType = "HASH"},
                    new KeySchemaElement {AttributeName = "Email", KeyType = "RANGE"}
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 10,
                    WriteCapacityUnits = 5
                }
            };

            ddbClient.CreateTable(request);
            // Pause until the table is active
            WaitForStatus(ddbClient, tableName, "ACTIVE");
            Console.WriteLine("Table created and active.");
        }


        public virtual string GetTableStatus(AmazonDynamoDBClient ddbClient, string tableName)
        {
            TableDescription tableDescription = GetTableDescription(ddbClient, tableName);
            if (tableDescription != null)
            {
                return tableDescription.TableStatus;
            }
            return "NOTFOUND";
        }

        public virtual TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
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


        public virtual void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
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
    }
}

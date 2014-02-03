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
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AwsLabs
{
    class Program
    {
        #region Student Tasks
        /// <summary>
        /// The region endpoint to use. Select the region that is the same as the one containing the table you are using.
        /// </summary>
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;


        #endregion

        #region Non-Student Code (Lab plumbing)

        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        /// <summary>
        /// Controlling method for the lab.
        /// </summary>
        public static void Main()
        {
            using (var ddbClient = new AmazonDynamoDBClient(RegionEndpoint))
            {
                const string tableName = "Accounts";
                var accountItems = new List<Account>
                {
                    new Account
                    {
                        Company = "Amazon.com",
                        Email = "johndoe@amazon.com",
                        First = "John",
                        Last = "Doe",
                        Age = "33"
                    },
                    new Account
                    {
                        Company = "Asperatus Tech",
                        Email = "janedoe@amazon.com",
                        First = "Jane",
                        Last = "Doe",
                        Age = "24"
                    },
                    new Account
                    {
                        Company = "Amazon.com",
                        Email = "jimjohnson@amazon.com",
                        First = "Jim",
                        Last = "Johnson"
                    }
                };

                try
                {
                    // Verify that the table schema is as we expect, and correct any problems we find.
                    if (!ConfirmTableSchema(ddbClient, tableName))
                    {
                        OptionalLabCode.DeleteTable(ddbClient, tableName);
                        OptionalLabCode.BuildTable(ddbClient, tableName);
                    }

                    Console.WriteLine("Adding items to table.");
                    // Create an account item
                    foreach (Account account in accountItems)
                    {
                        LabCode.CreateAccountItem(ddbClient, tableName, account);
                        Console.WriteLine("Added item: {0}/{1}", account.Company, account.Email);
                    }

                    Console.WriteLine("Requesting matches for Company == Amazon.com");
                    QueryResponse response = LabCode.LookupByHashKey(ddbClient, tableName, "Amazon.com");
                    if (response != null && response.Count > 0)
                    {
                        // Record was found
                        foreach (var item in response.Items)
                        {
                            Console.WriteLine("Item Found-");
                            foreach (var attr in item)
                            {
                                Console.WriteLine("    {0} : {1}", attr.Key, attr.Key == "Age" ? attr.Value.N : attr.Value.S);
                            }
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("No matches found.");
                    }

                    // Conditionally update a record
                    Console.Write("Attempting update. ");
                    LabCode.UpdateIfMatch(ddbClient, tableName, "jimjohnson@amazon.com", "Amazon.com", "James", "Jim");
                    Console.WriteLine("Done.");
                }
                catch (Exception ex)
                {
                    LabUtility.DumpError(ex);
                }
                finally
                {
                    Console.WriteLine("Press <enter> to end.");
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Inspect the specified table and confirm that it will be able to serve as the lab target. Since the table for the lab is 
        /// built by hand, it's possible that it may be in an unexpected state. 
        /// </summary>
        /// <param name="ddbClient">The client object.</param>
        /// <param name="tableName">The name of the table to inspect.</param>
        /// <returns>True if the schema is correct. False, if it's incorrect, doesn't exist, or isn't available.</returns>
        private static bool ConfirmTableSchema(AmazonDynamoDBClient ddbClient, string tableName)
        {
            Console.WriteLine("Confirming table schema.");
            TableDescription tableDescription = OptionalLabCode.GetTableDescription(ddbClient, tableName);

            if (tableDescription==null)
            {
                Console.WriteLine("Table does not exist.");
                // Can't match the schema if the table isn't there.
                return false;
            }
            if (!tableDescription.TableStatus.Equals("ACTIVE"))
            {
                Console.WriteLine("Table is not active.");
                return false;
            }

            if (tableDescription.AttributeDefinitions == null || tableDescription.KeySchema == null)
            {
                Console.WriteLine("Schema doesn't match.");
                return false;
            }
            foreach (AttributeDefinition attributeDefinition in tableDescription.AttributeDefinitions)
            {
                switch (attributeDefinition.AttributeName)
                {
                    case "Company":
                    case "Email":
                    case "First":
                    case "Last":
                        if (!attributeDefinition.AttributeType.Equals("S"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            Console.WriteLine("{0} attribute is wrong type in attribute definition.", attributeDefinition.AttributeName);
                            return false;
                        }
                        break;
                    case "Age":
                        if (!attributeDefinition.AttributeType.Equals("N"))
                        {
                            Console.WriteLine("Age attribute is wrong type in attribute definition.");
                            // We have a matching attribute, but the type is wrong.
                            return false;
                        }
                        break;
                }
            }
            // If we've gotten here, the attributes are good. Now check the key schema.
            if (tableDescription.KeySchema.Count != 2)
            {
                Console.WriteLine("Wrong number of elements in the key schema.");
                return false;
            }
            foreach (KeySchemaElement keySchemaElement in tableDescription.KeySchema)
            {
                switch (keySchemaElement.AttributeName)
                {
                    case "Company":
                        if (!keySchemaElement.KeyType.Equals("HASH"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            Console.WriteLine("Company attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    case "Email":
                        if (!keySchemaElement.KeyType.Equals("RANGE"))
                        {
                            // We have a matching attribute, but the type is wrong.
                            Console.WriteLine("Email attribute is wrong type in key schema.");
                            return false;
                        }
                        break;
                    default:
                        Console.WriteLine("Unexpected attribute ({0}) in the key schema.", keySchemaElement.AttributeName);
                        return false;
                }
                    
            }
            Console.WriteLine("Table schema is as expected.");
            // We've passed our checks.
            return true;
            
        }

        #endregion

    }

    #region Non-Student Code (Account class)
    /// <summary>
    /// Class used to represent the items in the table.
    /// </summary>
    public class Account
    {
        public string Email { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Age { get; set; }
        public string Company { get; set; }

        public Account()
        {
            Age = String.Empty;
            Last = String.Empty;
            First = String.Empty;
            Email = String.Empty;
            Company = String.Empty;
        }
    }
    #endregion


}
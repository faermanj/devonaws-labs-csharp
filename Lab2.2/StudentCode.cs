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
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     Create a DynamoDB item from the values specified in the account parameter. The names of the attributes in
        ///     the item should match the corresponding property names in the Account object. Don't add attributes for fields
        ///     in the Account object that are empty (hint: use String.IsNullOrEmpty() method on the value).
        ///     Since the Company and Email attributes are part of the table key, those will always be provided in
        ///     the Account object when this method is called.
        ///     Important: Even though the Account.Age property is passed to you as a string, store it in the  DynamoDB item
        ///     as a numerical value.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to add the items to.</param>
        /// <param name="account">The Account object containing the data to add.</param>
        /// <remarks>The purpose of this task is to give you experience constructing request objects for interacting with DynamoDB.</remarks>
        public override void CreateAccountItem(AmazonDynamoDBClient ddbClient, string tableName, Account account)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.CreateAccountItem(ddbClient, tableName, account);
        }

        /// <summary>
        ///     Construct a query request using the criteria specified and return the result object.
        ///     Hint: Use the Query() method of the client object.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="company">The company name to search for.</param>
        /// <remarks>The purpose of this task is to give you experience constructing request objects for interacting with DynamoDB.</remarks>
        /// <returns>The object containing the query results.</returns>
        public override QueryResponse LookupByHashKey(AmazonDynamoDBClient ddbClient, string tableName, string company)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.LookupByHashKey(ddbClient, tableName, company);
        }

        /// <summary>
        ///     Find items in the table matching the company and email parameter values. Set the value for the First attribute to
        ///     the firstNameTarget parameter value only if the attribute matches the firstNameMatch parameter value.
        ///     Hint: This can be accomplished with a single request using the UpdateItem() method of the client object.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table containing the items.</param>
        /// <param name="email">The value to match for the Email attribute.</param>
        /// <param name="company">The value to match for the Company attribute.</param>
        /// <param name="firstNameTarget">The value to put in the First attribute of the item if a match is found.</param>
        /// <param name="firstNameMatch">The value to match in the First attribute of the item.</param>
        /// <remarks>The purpose of this task is to give you experience constructing request objects for interacting with DynamoDB.</remarks>
        public override void UpdateIfMatch(AmazonDynamoDBClient ddbClient, string tableName, string email,
            string company,
            string firstNameTarget, string firstNameMatch)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.UpdateIfMatch(ddbClient, tableName, email, company, firstNameTarget, firstNameMatch);
        }

        #region Optional Tasks

        /// <summary>
        ///     Request the table description for the specified table and return it to the caller.
        ///     Hint: Use the DescribeTable operation.
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <remarks>
        ///     The purpose of this task is to gain experience inspecting the structure of DynamoDB tables.
        /// </remarks>
        /// <returns>The table description object. null if the table wasn't found.</returns>
        public override TableDescription GetTableDescription(AmazonDynamoDBClient ddbClient, string tableName)
        {
            return base.GetTableDescription(ddbClient, tableName);
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
        /// <exception cref="System.TimeoutException">Thrown when the timeout is reached.</exception>
        public override void WaitForStatus(AmazonDynamoDBClient ddbClient, string tableName, string status,
            DateTime? timeout = null)
        {
            base.WaitForStatus(ddbClient, tableName, status, timeout);
        }

        /// <summary>
        ///     Create the table that is used in this lab. Don't return from this method until the table state
        ///     is "ACTIVE". Hint: Call the WaitForStatus() method that you implemented above in order to wait.
        ///     Build the table to match these parameters:
        ///     --Attributes - "Company" a string, and "Email" also a string
        ///     --Hash Key Attribute - "Company"
        ///     --Range Key Attribute - "Email"
        ///     --Provisioned Capacity - 5 Reads/5 Writes
        ///     This method will be called by the lab controller if it determines that the table needs to be
        ///     rebuilt (ex. the schema doesn't match our expectations).
        /// </summary>
        /// <param name="ddbClient">The DynamoDB client object.</param>
        /// <param name="tableName">The name of the table to create.</param>
        /// <remarks>
        ///     The purpose of this task is to give you an opportunity to challenge yourself by discovering
        ///     how complete a complex exercise that wasn't discussed in class.
        /// </remarks>
        public override void BuildTable(AmazonDynamoDBClient ddbClient, string tableName)
        {
            base.BuildTable(ddbClient, tableName);
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
            base.DeleteTable(ddbClient, tableName);
        }

        #endregion
    }
}

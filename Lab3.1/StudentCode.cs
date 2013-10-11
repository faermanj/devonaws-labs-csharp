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
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class StudentCode : SolutionCode
    {
        /// <summary>
        ///     Create an SQS queue using the queue name provided and return the URL for the new queue.
        ///     Hint: Use the CreateQueue() method of the client object. The URL is in the response.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueName">The name of the queue to create.</param>
        /// <returns>The URL of the newly created queue.</returns>
        /// <remarks>The purpose of this task is to give you experience with the most relevant aspects of queue creation.</remarks>
        public override string CreateQueue(AmazonSQSClient sqsClient, string queueName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.CreateQueue(sqsClient, queueName);
        }

        /// <summary>
        ///     Query the SQS service for the ARN of the specified queue and return it.
        ///     Hint: Use the GetQueueAttributes() method of the client object. The attribute to request is named QueueArn.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueUrl">The URL for the queue to inspect.</param>
        /// <returns>A string containing the ARN for the queue.</returns>
        /// <remarks>
        ///     The purpose of this task is to give you experience requesting specific attributes of a queue and to make sure you
        ///     are comfortable identifying the ARN for it.
        /// </remarks>
        public override string GetQueueArn(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.GetQueueArn(sqsClient, queueUrl);
        }

        /// <summary>
        ///     Create an SNS topic and return the ARN for the newly created topic.
        ///     Hint: Use the CreateTopic() method of the client object. The ARN for the topic is contained in the response.
        /// </summary>
        /// <param name="snsClient">The SNS client object.</param>
        /// <param name="topicName">The topic name to create.</param>
        /// <returns>The ARN for the newly created topic.</returns>
        public override string CreateTopic(AmazonSimpleNotificationServiceClient snsClient, string topicName)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.CreateTopic(snsClient, topicName);
        }

        /// <summary>
        ///     Create an SNS subscription that publishes notifications to an SQS queue.
        ///     Hint: Use the Subscribe() method of the client object. The subscription endpoint is provided to you in the queueArn
        ///     parameter.
        /// </summary>
        /// <param name="snsClient">The SNS client object.</param>
        /// <param name="queueArn">The ARN for the queue that will be used as the subscription endpoint.</param>
        /// <param name="topicArn">The ARN for the topic to subscribe to.</param>
        /// <remarks>The purpose of this task is to familiarize you with the components that make up a subscription request.</remarks>
        public override void CreateSubscription(AmazonSimpleNotificationServiceClient snsClient, string queueArn,
            string topicArn)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.CreateSubscription(snsClient, queueArn, topicArn);
        }

        /// <summary>
        ///     Publish a message to the specified SNS topic.
        ///     Hint: Use the Publish() method of the client object.
        /// </summary>
        /// <param name="snsClient">The SNS client object.</param>
        /// <param name="topicArn">The ARN for the topic to post the message to.</param>
        /// <param name="subject">The subject of the message to publish.</param>
        /// <param name="message">The body of the message to publish.</param>
        public override void PublishTopicMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArn,
            string subject,
            string message)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.PublishTopicMessage(snsClient, topicArn, subject, message);
        }

        /// <summary>
        ///     Post a message to the specified queue.
        ///     Hint: Use the SendMessage() method of the client object.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueUrl">The URL for the queue to place the message in.</param>
        /// <param name="messageText">The body of the message to place in the queue.</param>
        public override void PostToQueue(AmazonSQSClient sqsClient, string queueUrl, string messageText)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.PostToQueue(sqsClient, queueUrl, messageText);
        }

        /// <summary>
        ///     Read up to 10 messages from the specified SQS queue with one request.
        ///     Hint: Use the ReceiveMessage() method of the client object. In the request, set the maximum number of messages to
        ///     10.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueUrl">The URL of the queue containing the messages.</param>
        /// <returns>A list of messages from the queue.</returns>
        /// <remarks>
        ///     The purpose of this task is to give you experience reading messages from a queue and to show you that they come
        ///     back one at a time by default.
        /// </remarks>
        public override List<Message> ReadMessages(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            return base.ReadMessages(sqsClient, queueUrl);
        }

        /// <summary>
        ///     Delete the specified message from the specified queue.
        ///     Hint: Use the DeleteMessage() method of the client object.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueUrl">The URL of the queue containing the message.</param>
        /// <param name="receiptHandle">The receipt handle of the message to delete.</param>
        /// <remarks>The purpose of this task is to give you experience deleting messages from a queue.</remarks>
        public override void RemoveMessage(AmazonSQSClient sqsClient, string queueUrl, string receiptHandle)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.RemoveMessage(sqsClient, queueUrl, receiptHandle);
        }

        /// <summary>
        ///     Delete all subscriptions to the specified SNS topic.
        ///     Hint: Call ListSubscriptionsByTopic() on the client object to get all fo the subscriptions and loop through them
        ///     calling the client object's Unsubscribe() method with details of each subscription.
        /// </summary>
        /// <param name="snsClient">The SNS client object.</param>
        /// <param name="topicArn">The SNS topic to remove the subscriptions from.</param>
        /// <remarks>
        ///     The purpose of this task is to give you experience cleaning up unused resources. Subscriptions must be deleted
        ///     before the notification topic can be deleted.
        /// </remarks>
        public override void DeleteSubscriptions(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.DeleteSubscriptions(snsClient, topicArn);
        }

        /// <summary>
        ///     Delete the specified SNS topic.
        ///     Hint: Use the DeleteTopic() method of the client object.
        /// </summary>
        /// <param name="snsClient">The SNS client object.</param>
        /// <param name="topicArn">The ARN of the topic to delete.</param>
        /// <remarks>The purpose of this task is to give you experience cleaning up unused resources.</remarks>
        public override void DeleteTopic(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.DeleteTopic(snsClient, topicArn);
        }

        /// <summary>
        ///     Delete the specified SQS queue.
        ///     Hint: Use the DeleteQueue() method of the client object.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueUrl">The URL of the queue to delete.</param>
        /// <remarks>The purpose of this task is to give you experience cleaning up unused resources.</remarks>
        public override void DeleteQueue(AmazonSQSClient sqsClient, string queueUrl)
        {
            //TODO: Replace this call to the base class with your own method implementation.
            base.DeleteQueue(sqsClient, queueUrl);
        }

        #region Optional Tasks

        /// <summary>
        ///     Grant permission allowing the provided SNS topic to publish messages to your queue. To accomplish this you will
        ///     need to create a properly formed policy statement and assign it to the Policy attribute of the queue. You will
        ///     need to do some research to get this right.
        /// </summary>
        /// <param name="sqsClient">The SQS client object.</param>
        /// <param name="queueArn">The ARN defining the queue. This is used as the Resource in the policy statement.</param>
        /// <param name="queueUrl">
        ///     The URL for the queue. This is used to identify the queue for the purpose of updating its Policy
        ///     attribute.
        /// </param>
        /// <param name="topicArn">
        ///     The ARN for the topic that will publish to the queue. This will be used as a source ARN
        ///     Condition in the policy statement.
        /// </param>
        /// <remarks>
        ///     The purpose of this task is to challenge you to explore beyond what was covered in the class, but in a safe
        ///     and supportive environment where you can discuss the challenge freely.
        /// </remarks>
        public override void GrantNotificationPermission(AmazonSQSClient sqsClient, string queueArn, string queueUrl,
            string topicArn)
        {
            base.GrantNotificationPermission(sqsClient, queueArn, queueUrl, topicArn);
        }

        #endregion
    }
}

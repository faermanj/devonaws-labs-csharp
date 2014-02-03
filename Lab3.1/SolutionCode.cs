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
using Amazon.Auth.AccessControlPolicy;
using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class SolutionCode : IOptionalLabCode, ILabCode
    {
        public virtual string CreateQueue(AmazonSQSClient sqsClient, string queueName)
        {
            string queueUrl;

            // Create the request
            var createQueueRequest = new CreateQueueRequest {QueueName = queueName};

            // Submit the request
            CreateQueueResponse createQueueResponse = sqsClient.CreateQueue(createQueueRequest);

            // Return the URL for the newly created queue
            queueUrl = createQueueResponse.QueueUrl;
            return queueUrl;
        }


        public virtual string GetQueueArn(AmazonSQSClient sqsClient, string queueUrl)
        {
            string queueArn;
            // Construct a GetQueueAttributesRequest for the URL to retrieve the ARN
            var getQueueAttributesRequest = new GetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                AttributeNames =
                {
                    "QueueArn"
                }
            };

            // Submit the request
            GetQueueAttributesResponse getQueueAttributesResponse =
                sqsClient.GetQueueAttributes(getQueueAttributesRequest);

            // Add the discovered ARN to the queueArnList variable.
            queueArn = getQueueAttributesResponse.QueueARN;
            return queueArn;
        }


        public virtual string CreateTopic(AmazonSimpleNotificationServiceClient snsClient, string topicName)
        {
            string topicArn;
            // Create the request
            var createTopicRequest = new CreateTopicRequest
            {
                Name = topicName
            };
            // Submit the request
            CreateTopicResponse topicResponse = snsClient.CreateTopic(createTopicRequest);

            // Return the ARN
            topicArn = topicResponse.TopicArn;
            return topicArn;
        }


        public virtual void CreateSubscription(AmazonSimpleNotificationServiceClient snsClient, string queueArn,
            string topicArn)
        {
            // Create the request
            var subscriptionRequest = new SubscribeRequest
            {
                Endpoint = queueArn,
                Protocol = "sqs",
                TopicArn = topicArn
            };
            // Create the subscription
            snsClient.Subscribe(subscriptionRequest);
        }


        public virtual void PublishTopicMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArn,
            string subject, string message)
        {
            // Create the request
            var publishRequest = new PublishRequest
            {
                Subject = subject,
                Message = message,
                TopicArn = topicArn
            };

            // Submit the request
            snsClient.Publish(publishRequest);
        }


        public virtual void PostToQueue(AmazonSQSClient sqsClient, string queueUrl, string messageText)
        {
            // Create the request
            var sendMessageRequest = new SendMessageRequest
            {
                MessageBody = messageText,
                QueueUrl = queueUrl
            };

            //Send message to queue
            sqsClient.SendMessage(sendMessageRequest);
        }

        public virtual List<Message> ReadMessages(AmazonSQSClient sqsClient, string queueUrl)
        {
            // Create the request
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10
            };
            // Submit the request and return the response
            ReceiveMessageResponse resp = sqsClient.ReceiveMessage(receiveMessageRequest);
            return resp.Messages;
        }

        public virtual void RemoveMessage(AmazonSQSClient sqsClient, string queueUrl, string receiptHandle)
        {
            // Create the request
            var deleteMessageRequest = new DeleteMessageRequest
            {
                ReceiptHandle = receiptHandle,
                QueueUrl = queueUrl
            };
            // Submit the request
            sqsClient.DeleteMessage(deleteMessageRequest);
        }


        public virtual void DeleteSubscriptions(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            var listSubscriptionsByTopicRequest = new ListSubscriptionsByTopicRequest
            {
                TopicArn = topicArn
            };

            ListSubscriptionsByTopicResponse listSubscriptionsByTopicResponse =
                snsClient.ListSubscriptionsByTopic(listSubscriptionsByTopicRequest);

            foreach (
                Subscription subscription in
                    listSubscriptionsByTopicResponse.Subscriptions)
            {
                var unsubscribeRequest = new UnsubscribeRequest
                {
                    SubscriptionArn = subscription.SubscriptionArn
                };
                snsClient.Unsubscribe(unsubscribeRequest);
            }
        }


        public virtual void DeleteTopic(AmazonSimpleNotificationServiceClient snsClient, string topicArn)
        {
            // Create the request
            var deleteTopicRequest = new DeleteTopicRequest
            {
                TopicArn = topicArn
            };

            snsClient.DeleteTopic(deleteTopicRequest);
        }


        public virtual void DeleteQueue(AmazonSQSClient sqsClient, string queueUrl)
        {
            var deleteQueueRequest = new DeleteQueueRequest
            {
                QueueUrl = queueUrl
            };
            // Delete the queue
            sqsClient.DeleteQueue(deleteQueueRequest);
        }

        public virtual void GrantNotificationPermission(AmazonSQSClient sqsClient, string queueArn, string queueUrl,
            string topicArn)
        {
            // Create a policy to allow the queue to receive notifications from the SNS topic
            var policy = new Policy("SubscriptionPermission")
            {
                Statements =
                {
                    new Statement(Statement.StatementEffect.Allow)
                    {
                        Actions = {SQSActionIdentifiers.SendMessage},
                        Principals = {new Principal("*")},
                        Conditions = {ConditionFactory.NewSourceArnCondition(topicArn)},
                        Resources = {new Resource(queueArn)}
                    }
                }
            };

            var attributes = new Dictionary<string, string>();
            attributes.Add("Policy", policy.ToJson());

            // Create the request to set the queue attributes for policy
            var setQueueAttributesRequest = new SetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                Attributes = attributes
            };

            // Set the queue policy
            sqsClient.SetQueueAttributes(setQueueAttributesRequest);
        }
    }
}

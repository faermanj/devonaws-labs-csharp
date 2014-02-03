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
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace AwsLabs
{
    internal class Program
    {
        #region Student Tasks

        /// <summary>
        ///     The region endpoint to use. Change this if instructed to do so by your trainer.
        /// </summary>
        private static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.USEast1;

        #endregion

        #region Non-Student Code

        private static readonly ILabCode LabCode = new StudentCode();
        private static readonly IOptionalLabCode OptionalLabCode = new StudentCode();

        public static void Main(string[] args)
        {
            try
            {
                using (var snsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint))
                {
                    using (var sqsClient = new AmazonSQSClient(RegionEndpoint))
                    {
                        const string queueName = "Notifications";
                        const string topicName = "ClassroomEvent";

                        // Creating the queue will fail if we've just deleted it and are recreating it 
                        // which is a possibility if you're tracking down a code error. If that happens, 
                        // pause and retry for up to a minute. 
                        Console.WriteLine("Creating {0} queue.", queueName);

                        bool retry = true, notified = false;
                        DateTime start = DateTime.Now;
                        string queueUrl = "";

                        while (retry)
                        {
                            try
                            {
                                // Create an SQS queue
                                queueUrl = LabCode.CreateQueue(sqsClient, queueName);
                                retry = false;
                            }
                            catch (AmazonSQSException ex)
                            {
                                if (!ex.ErrorCode.Equals("AWS.SimpleQueueService.QueueDeletedRecently"))
                                {
                                    // This is an unexpected error, so waiting and retrying may not help.
                                    // Just rethrow.
                                    throw;
                                }

                                if (DateTime.Now < (start + TimeSpan.FromSeconds(60)))
                                {
                                    if (!notified)
                                    {
                                        Console.WriteLine(
                                            "The attempt to recreate the queue failed because the queue was deleted too\nrecently. Waiting and retrying for up to 1 minute.");
                                        notified = true;
                                    }
                                    // Timeout hasn't expired yet so wait and retry in 5 seconds.
                                    Console.Write(".");
                                    Thread.Sleep(TimeSpan.FromSeconds(5));
                                }
                                else
                                {
                                    Console.WriteLine("Retry timeout expired. Aborting.");
                                    throw;
                                }
                            }
                        }
                        if (notified)
                        {
                            Console.WriteLine("Recovered.");
                        }
                        Console.WriteLine("URL for new queue:\n    {0}", queueUrl);
                        // List SQS queues
                        Console.WriteLine("Getting ARN for {0} queue.", queueName);
                        string queueArn = LabCode.GetQueueArn(sqsClient, queueUrl);
                        Console.WriteLine("ARN for queue: {0}", queueArn);

                        // Create an SNS topic and get ARN
                        Console.WriteLine("Creating {0} topic.", topicName);
                        string topicArn = LabCode.CreateTopic(snsClient, topicName);
                        Console.WriteLine("New topic ARN: {0}", topicArn);

                        Console.WriteLine("Granting the notification topic permission to post in the queue.");
                        OptionalLabCode.GrantNotificationPermission(sqsClient, queueArn, queueUrl, topicArn);
                        Console.WriteLine("Permission granted.");

                        // Create an SNS subscription
                        Console.WriteLine("Creating SNS subscription.");
                        LabCode.CreateSubscription(snsClient, queueArn, topicArn);
                        Console.WriteLine("Subscription created.");

                        // Publish message to topic
                        var messageText = "This is the SNS topic notification body.";
                        var messageSubject = "SNSTopicNotification";

                        Console.WriteLine("Publishing SNS topic notification.");
                        LabCode.PublishTopicMessage(snsClient, topicArn, messageSubject, messageText);
                        Console.WriteLine("Notification published.");

                        // Send a message to the "Notifications" queue
                        messageText = "This is the message posted to the queue directly.";
                        Console.WriteLine("Posting message to queue directly.");
                        LabCode.PostToQueue(sqsClient, queueUrl, messageText);
                        Console.WriteLine("Message posted.");

                        //Console.WriteLine(">> PAUSING FOR A MOMENT <<");
                        //Thread.Sleep(TimeSpan.FromSeconds(5));

                        // Read messages from queue
                        Console.WriteLine("Reading messages from queue.");

                        List<Message> messages = LabCode.ReadMessages(sqsClient, queueUrl);
                        // We expect two messages here
                        if (messages.Count < 2)
                        {
                            // Try to read again and see if we've picked up the missing message(s).
                            messages.AddRange(LabCode.ReadMessages(sqsClient, queueUrl));
                            if (messages.Count < 2)
                            {
                                Console.WriteLine(
                                    ">>WARNING<< We didn't receive the expected number of messages. Investigate.");
                            }
                            else
                            {
                                Console.WriteLine(
                                    "\n===============================================================================");
                                Console.WriteLine(
                                    "PROBLEM: ReadMessages() had to be called twice to collect all the messages.");
                                Console.WriteLine(
                                    "         Did you remember to set the MaxNumberOfMessages property in the ");
                                Console.WriteLine("         ReceiveMessageRequest object?");
                                Console.WriteLine(
                                    "===============================================================================\n");
                            }
                        }
                        PrintAndRemoveMessagesInResponse(sqsClient, messages, queueUrl);

                        // Locate and delete the SNS subscription
                        Console.WriteLine("Removing provisioned resources.");
                        LabCode.DeleteSubscriptions(snsClient, topicArn);
                        Console.WriteLine("Subscriptions removed.");

                        // Delete the SNS Topic
                        LabCode.DeleteTopic(snsClient, topicArn);
                        Console.WriteLine("Topic deleted.");
                        // Locate the previously created queue and delete
                        LabCode.DeleteQueue(sqsClient, queueUrl);
                        Console.WriteLine("Queue deleted.");
                    }
                }
            }
            catch (Exception ex)
            {
                LabUtility.DumpError(ex);
            }
            finally
            {
                Console.WriteLine("\n\nPress <enter> to end.");
                Console.ReadLine();
            }
        }


        // Print the message contents to the console window
        private static void PrintAndRemoveMessagesInResponse(AmazonSQSClient sqsClient, List<Message> messages,
            string queueUrl)
        {
            foreach (var message in messages)
            {
                Console.WriteLine("\nQueue Message:");

                Console.WriteLine("\tMessageId : {0}", message.MessageId);
                Console.WriteLine("\tMD5OfBody : {0}", message.MD5OfBody);
                // pull out newline characters so it displays better.
                message.Body = message.Body.Replace("\n", "\\n");
                if (message.Body.Length > 50)
                {
                    Console.WriteLine("\tBody : {0}...", message.Body.Substring(0, 50));
                }
                else
                {
                    Console.WriteLine("\tBody : {0}", message.Body);
                }

                if (message.Attributes.Count > 0)
                {
                    Console.WriteLine("\tMessage Attributes");
                    foreach (var entry in message.Attributes)
                    {
                        Console.WriteLine("\t\t{0} : {1}", entry.Key, entry.Value);
                    }
                }

                Console.WriteLine("\nDeleting message.");
                LabCode.RemoveMessage(sqsClient, queueUrl, message.ReceiptHandle);
                Console.WriteLine("Message deleted.");
            }
        }

        #endregion
    }
}

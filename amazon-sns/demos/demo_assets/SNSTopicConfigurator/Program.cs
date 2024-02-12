using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using System;
using System.Collections.Generic;
using System.IO;

namespace SNSTopicConfigurator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var region = RegionEndpoint.GetBySystemName("YOUR AWS REGION");
            string AWSAccessKey = "YOUR AWS ACCESS KEY";
            string AWSSecretKey = "YOUR AWS SECRET KEY";

            string topicArn = "YOUR TOPIC ARN";

            string successFeedbackRoleArn = "YOUR SUCCESS FEEDBACK ROLE ARN";
            string successFeedbackSampleRate = "100";
            string failureFeedbackRoleArn = "YOUR FAILURE FEEDBACK ROLE ARN";

            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient(
                new BasicAWSCredentials(
                    AWSAccessKey,
                    AWSSecretKey),
                region
            );

            Dictionary<string, string> attributes = new Dictionary<string, string>()
            {
                { "ApplicationSuccessFeedbackRoleArn", successFeedbackRoleArn },
                { "ApplicationSuccessFeedbackSampleRate", successFeedbackSampleRate },
                { "ApplicationFailureFeedbackRoleArn", failureFeedbackRoleArn },

                { "HTTPSuccessFeedbackRoleArn", successFeedbackRoleArn },
                { "HTTPSuccessFeedbackSampleRate", successFeedbackSampleRate },
                { "HTTPFailureFeedbackRoleArn", failureFeedbackRoleArn },

                { "LambdaSuccessFeedbackRoleArn", successFeedbackRoleArn },
                { "LambdaSuccessFeedbackSampleRate", successFeedbackSampleRate },
                { "LambdaFailureFeedbackRoleArn", failureFeedbackRoleArn },

                { "SQSSuccessFeedbackRoleArn", successFeedbackRoleArn },
                { "SQSSuccessFeedbackSampleRate", successFeedbackSampleRate },
                { "SQSFailureFeedbackRoleArn", failureFeedbackRoleArn },

                { "DeliveryPolicy", File.ReadAllText("deliveryRetryPolicy.json")}
            };

            Console.WriteLine($"Setting attributes for topic: '{topicArn}'");

            foreach (var attribute in attributes)
            {
                Console.WriteLine($"Setting attribute '{attribute.Key}'");
                snsClient.SetTopicAttributesAsync(
                    topicArn: topicArn,
                    attributeName: attribute.Key,
                    attributeValue: attribute.Value
                ).GetAwaiter().GetResult();
            }

            Console.WriteLine("All attributes set");
            Console.ReadLine();
        }
    }
}

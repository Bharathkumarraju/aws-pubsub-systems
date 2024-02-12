using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using CloudNotes.SQSEventProcessor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudNotes.SQSEventProcessor.Services
{
    public class SQSEventConsumer : IEventConsumer
    {
        private readonly ILogger<SQSEventConsumer> _logger;
        private readonly AmazonSQSClient _sqsClient;
        private readonly SQSSettings _sqsSettings;
        private bool _running = false;

        public SQSEventConsumer(IOptions<SQSSettings> sqsSettings, ILogger<SQSEventConsumer> logger)
        {
            _logger = logger;

            try
            {
                _sqsSettings = sqsSettings.Value;

                RegionEndpoint region = RegionEndpoint.GetBySystemName(_sqsSettings.AWSRegion);

                _sqsClient = new AmazonSQSClient(
                    new BasicAWSCredentials(
                        _sqsSettings.AWSAccessKey,
                        _sqsSettings.AWSSecretKey),
                    region);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't create an instance of SQSEventPublisher");
                throw;
            }
        }

        public void Start()
        {
            _logger.LogInformation("Starting event consumer");
            _running = true;

            _logger.LogInformation($"Collecting messages from queue '{_sqsSettings.QueueUrl}'");

            Task.Run(async () =>
            {
                while (_running)
                {
                    _logger.LogInformation("Polling for messages");

                    // Receive messages
                    try
                    {
                        ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest(_sqsSettings.QueueUrl);

                        receiveMessageRequest.MaxNumberOfMessages = _sqsSettings.MaxNumberOfMessagesPerRequest;
                        receiveMessageRequest.WaitTimeSeconds = _sqsSettings.ReceiveMessageWaitTimeInSeconds;
                        receiveMessageRequest.MessageAttributeNames.Add("Publisher");

                        ReceiveMessageResponse receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);

                        // Process messages
                        Parallel.ForEach(receiveMessageResponse.Messages, async (Message message) =>
                        {
                            string publisher = message.MessageAttributes["Publisher"].StringValue;

                            Event cloudNotesEvent = JsonConvert.DeserializeObject<Event>(message.Body);

                            _logger.LogInformation(" [x] Received {0} from '{1}'", cloudNotesEvent.NoteId, publisher);

                            // TODO: Do interesting work based on the new message

                            try
                            {
                                // Delete the message after processing
                                DeleteMessageRequest deleteMessageRequest = new DeleteMessageRequest(
                                    _sqsSettings.QueueUrl,
                                    message.ReceiptHandle
                                );

                                await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogCritical(ex, "Couldn't delete an event from SQS");
                                throw;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Couldn't receive events from SQS");
                        throw;
                    }

                    // Wait before polling again
                    Thread.Sleep(TimeSpan.FromSeconds(_sqsSettings.PollingIntervalInSeconds));
                }
            });

            _logger.LogInformation("Event consumer started");
        }

        public void Stop()
        {
            _logger.LogInformation("Stopping event consumer");
            _running = false;
            _logger.LogInformation("Event consumer stopped");
        }
    }
}

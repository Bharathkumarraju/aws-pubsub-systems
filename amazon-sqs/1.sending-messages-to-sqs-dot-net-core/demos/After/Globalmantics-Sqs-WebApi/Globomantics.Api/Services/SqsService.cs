using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Globomantics.Api.Models;

namespace Globomantics.Api.Services
{
    public class SqsService : ISqsService
    {
        private readonly IAmazonSQS _sqsClient;

        public SqsService(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }

        public async Task<SendMessageResponse> SendMessageToSqsQueue(TicketRequest request)
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = "https://sqs.us-east-1.amazonaws.com/117937980928/TicketRequest",
                MessageBody = request.Serialize(request)
            };

            var sendMessageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest);

            return sendMessageResponse;
        }
    }
}

using System.Threading.Tasks;
using Amazon.SQS.Model;
using Globomantics.Api.Models;

namespace Globomantics.Api.Services
{
    public interface ISqsService
    {
        Task<SendMessageResponse> SendMessageToSqsQueue(TicketRequest request);
    }
}
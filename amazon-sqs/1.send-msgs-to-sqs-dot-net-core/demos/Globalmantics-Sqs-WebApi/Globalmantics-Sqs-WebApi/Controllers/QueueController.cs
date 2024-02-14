using System.Threading.Tasks;
using Globomantics.Api.Models;
using Globomantics.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.Api.Controllers
{
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly ISqsService _sqsService;

        public QueueController(ISqsService sqsService)
        {
            _sqsService = sqsService;
        }

        [HttpPost]
        [Route("sendmessage")]
        public async Task<IActionResult> SendMessage(TicketRequest request)
        {
            var response = await _sqsService.SendMessageToSqsQueue(request);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}

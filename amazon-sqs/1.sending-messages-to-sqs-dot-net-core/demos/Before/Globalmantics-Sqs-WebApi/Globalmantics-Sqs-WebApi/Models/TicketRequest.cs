using System;

namespace Globalmantics_Sqs_WebApi.Models
{
    public class TicketRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }
}

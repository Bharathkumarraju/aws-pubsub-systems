using System;

namespace CloudNotes.SQSEventProcessor.Models
{
    public class Event
    {
        public EventType EventType { get; set; }
        public Guid NoteId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Username { get; set; }
    }
}

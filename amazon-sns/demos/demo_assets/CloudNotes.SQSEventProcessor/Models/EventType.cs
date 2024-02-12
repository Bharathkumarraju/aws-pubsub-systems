namespace CloudNotes.SQSEventProcessor.Models
{ 
    public enum EventType
    {
        NoteCreated,
        NoteEdited,
        NoteViewed,
        NoteDeleted
    }
}

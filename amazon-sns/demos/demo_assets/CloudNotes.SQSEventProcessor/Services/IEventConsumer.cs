namespace CloudNotes.SQSEventProcessor.Services
{
    public interface IEventConsumer
    {
        void Start();
        void Stop();
    }
}
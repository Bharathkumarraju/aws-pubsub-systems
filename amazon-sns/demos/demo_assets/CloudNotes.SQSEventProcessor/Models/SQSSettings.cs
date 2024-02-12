namespace CloudNotes.SQSEventProcessor.Models
{
    public class SQSSettings
    {
        public string AWSRegion { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string QueueUrl { get; set; }
        public int ReceiveMessageWaitTimeInSeconds { get; set; }
        public int PollingIntervalInSeconds { get; set; }
        public int MaxNumberOfMessagesPerRequest { get; set; }
    }
}

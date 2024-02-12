using Newtonsoft.Json;

namespace CloudNotes.Models.MobilePush
{
    public class MobilePushNotification
    { 
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        public MobilePushNotification(string title, string body)
        {
            Title = title;
            Body = body;
        }
    }
}

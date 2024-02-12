using Newtonsoft.Json;
using System.Collections.Generic;

namespace CloudNotes.Models.MobilePush
{
    public class FCMNotification
    {
        [JsonProperty("notification")]
        public MobilePushNotification Notification { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; }

        public FCMNotification()
        {
            Data = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

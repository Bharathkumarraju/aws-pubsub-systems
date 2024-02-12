using Android.App;
using Android.Content;
using Firebase.Messaging;
using Android.Support.V4.App;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using System.Threading.Tasks;
using System.Net;
using System.Linq;

namespace CloudNotes.AndroidApp
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class SNSFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnNewToken(string token)
        {
            SendRegistrationToServer(token).GetAwaiter().GetResult();
        }

        private async Task SendRegistrationToServer(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var region = RegionEndpoint.GetBySystemName(Settings.AwsRegion);
                var snsClient = new AmazonSimpleNotificationServiceClient(
                    new BasicAWSCredentials(
                        Settings.AwsAccessKey,
                        Settings.AwsSecretKey),
                    region
                );

                // create application endpoint
                var endpointCreationRequest = new CreatePlatformEndpointRequest
                {
                    Token = token,
                    PlatformApplicationArn = Settings.PlatformApplicationArn
                };

                var createEndpointResponse =
                    await snsClient.CreatePlatformEndpointAsync(
                        endpointCreationRequest);

                // create SNS subscription
                if (createEndpointResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    SubscribeRequest subscribeRequest = new SubscribeRequest()
                    {
                        TopicArn = Settings.TopicArn,
                        Protocol = "application",
                        Endpoint = createEndpointResponse.EndpointArn
                    };
                    subscribeRequest.Attributes
                        .Add("FilterPolicy", "{\"Publisher\": [{\"exists\": true}]}");

                    await snsClient.SubscribeAsync(subscribeRequest);
                }
            }
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            RemoteMessage.Notification notification = message.GetNotification();

            if (notification != null)
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                var pendingIntent =
                    PendingIntent.GetActivity(
                        this, 0, intent, PendingIntentFlags.OneShot);

                var notificationBuilder =
                    new NotificationCompat.Builder(this, Settings.ChannelId);
                notificationBuilder
                    .SetContentTitle(notification.Title)
                    .SetSmallIcon(Resource.Mipmap.ic_launcher_foreground)
                    .SetContentText(notification.Body)
                    .SetContentIntent(pendingIntent);

                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(0, notificationBuilder.Build());
            }
            else if (message.Data != null && message.Data.Any())
            {
                foreach (var item in message.Data)
                {
                    string key = item.Key;
                    string value = item.Value;

                    // Do something with the data
                }
            }
        }
    }
}
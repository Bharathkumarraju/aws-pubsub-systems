using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Android.Util;
using Android.Gms.Common;
using Firebase.Iid;

namespace CloudNotes.AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        internal const string TAG = "MainActivity";

        TextView msgText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            msgText = FindViewById<TextView>(Resource.Id.msgText);
            
            var logTokenButton = FindViewById<Button>(Resource.Id.logTokenButton);

            logTokenButton.Click += delegate {
                msgText.Text = FirebaseInstanceId.Instance.Token;
                Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            };

            IsPlayServicesAvailable();
            CreateNotificationChannel();
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = 
                GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    msgText.Text = 
                        GoogleApiAvailability.Instance.GetErrorString(resultCode);
                }
                else
                {
                    msgText.Text = "This device is not supported";
                    Finish();
                }
                return false;
            }

            msgText.Text = "Google Play Services is available.";
            return true;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channelName = "FCM Notifications";
            var channelDescription = "Firebase Cloud Messages appear in this channel";
            var channel = new NotificationChannel(
                Settings.ChannelId, 
                channelName, 
                NotificationImportance.Default)
            {
                Description = channelDescription
            };

            var notificationManager = 
                (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}


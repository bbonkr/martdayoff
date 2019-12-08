using System;
using System.IO;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Java.Lang;
using MartDayOff.Models;
using MartDayOff.Services;
using AndroidApp = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(MartDayOff.Droid.Services.LocalNotificationService))]
namespace MartDayOff.Droid.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        int _notificationIconId { get; set; }
        readonly DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal string _randomNumber;

        public static Intent GetLauncherActivity()
        {
            var packageName = Application.Context.PackageName;

            return Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
        }
        
        public LocalNotificationService()
        {
        }

        public event EventHandler<LocalNotificationReceivedEventArgs> Received;

        public event EventHandler<LocalNotificationRegisteredEventArgs> Registered;

        public event EventHandler<LocalNotificationUnregisteredEventArgs> Unregistered;

        public void Register(LocalNotification localNotification)
        {

            //long repeateDay = 1000 * 60 * 60 * 24;    
            long repeateForMinute = 60000; // In milliseconds   
            long totalMilliSeconds = (long)(localNotification.NotifyTime.ToUniversalTime() - _jan1st1970).TotalMilliseconds;
            if (totalMilliSeconds < JavaSystem.CurrentTimeMillis())
            {
                totalMilliSeconds = totalMilliSeconds + repeateForMinute;
            }

            var intent = CreateIntent(localNotification.Id);

            if (_notificationIconId != 0)
            {
                localNotification.IconId = _notificationIconId;
            }
            else
            {
                localNotification.IconId = Resource.Drawable.dayoff_notification_icon;
            }

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            Random generator = new Random();
            _randomNumber = generator.Next(100000, 999999).ToString("D6");

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            //alarmManager.SetRepeating(AlarmType.RtcWakeup, totalMilliSeconds, repeateForMinute, pendingIntent);
            //alarmManager.SetTime(totalMilliSeconds);
            alarmManager.SetExact(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);


            OnRegistered(new LocalNotificationRegisteredEventArgs(localNotification.Id));
        }

        public void Unregister(int id)
        {
            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();
            notificationManager.Cancel(id);

            OnUnregistered(new LocalNotificationUnregisteredEventArgs(id));
        }

        public void OnReceived(LocalNotificationReceivedEventArgs e)
        {
            Received?.Invoke(this, e);
        }

        public void OnRegistered(LocalNotificationRegisteredEventArgs e)
        {
            Registered?.Invoke(this, e);
        }

        public void OnUnregistered(LocalNotificationUnregisteredEventArgs e)
        {
            Unregistered?.Invoke(this, e);
        }

        private Intent CreateIntent(int id)
        {
            return new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction($"LocalNotification{id}");
        }

        private AlarmManager GetAlarmManager()
        {
            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            return alarmManager;
        }

        private string SerializeNotification(LocalNotification model)
        {
            var xmlSerializer = new XmlSerializer(model.GetType());
                
            using(var writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, model);

                return writer.ToString();
            }
        }
    }

    [BroadcastReceiver(Enabled = true, Label ="Local Notification Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {
        public const string Tag = "ScheduledAlarmHandler";

        public const string LocalNotificationKey = "LocalNotificatio";

        const string channelId = "default";
        const string channelName = "Default";
        const string channelDescription = "The default channel for notifications.";
        const int pendingIntentId = 0;

        bool channelInitialized = false;
        int messageId = -1;
        NotificationManager manager;

        public ScheduledAlarmHandler(): base()
        {
            CreateNotificationChannel();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (!channelInitialized)
            {
                CreateNotificationChannel();
            }

            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            var builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(notification.IconId)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))
                .SetAutoCancel(true);

            var resultIntent = LocalNotificationService.GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);

            Random random = new Random();
            int randomNumber = random.Next(9999 - 1000) + 1000;

            var resultPendingIntent =
                stackBuilder.GetPendingIntent(randomNumber, (int)PendingIntentFlags.Immutable);
            builder.SetContentIntent(resultPendingIntent);

            // Sending notification    
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(randomNumber, builder.Build());

            var localNotificationService =Xamarin.Forms.DependencyService.Get<ILocalNotificationService>();
            localNotificationService.OnReceived(new LocalNotificationReceivedEventArgs(notification.Id));            
        }

        private LocalNotification DeserializeNotification(string extra)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
            using (var reader = new StringReader(extra))
            {
                try
                {
                    var notification = xmlSerializer.Deserialize(reader) as LocalNotification;

                    return notification;
                }
                catch (System.Exception ex)
                {
                    Log.Error(Tag, ex.Message);
                    return null;
                }
                
            }
        }

        private void CreateNotificationChannel()
        {
            messageId = Xamarin.Essentials.Preferences.Get(PreferenceKeys.MessageId, 1);

            manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelNameJava = new Java.Lang.String(channelName);
                var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
                {
                    Description = channelDescription
                };
                manager.CreateNotificationChannel(channel);
            }

            channelInitialized = true;
        }
    }
}

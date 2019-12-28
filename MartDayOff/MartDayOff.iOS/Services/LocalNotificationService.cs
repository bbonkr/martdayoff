using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using MartDayOff.Models;
using MartDayOff.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(MartDayOff.iOS.Services.LocalNotificationService))]
namespace MartDayOff.iOS.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        const string NotificationKey = "LocalNotificationKey";

        public event EventHandler<LocalNotificationReceivedEventArgs> Received;
        public event EventHandler<LocalNotificationRegisteredEventArgs> Registered;
        public event EventHandler<LocalNotificationUnregisteredEventArgs> Unregistered;

        public void OnReceived(LocalNotificationReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnRegistered(LocalNotificationRegisteredEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnUnregistered(LocalNotificationUnregisteredEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Register(LocalNotification localNotification)
        {
            var notification = new UILocalNotification
            {
                AlertTitle = localNotification.Title,
                AlertBody = localNotification.Body,
                SoundName = UILocalNotification.DefaultSoundName,
                FireDate = localNotification.NotifyTime.ToNSDate(),
                RepeatInterval = NSCalendarUnit.Minute,

                UserInfo = NSDictionary.FromObjectAndKey(NSObject.FromObject(localNotification.Id), NSObject.FromObject(NotificationKey)),
            };

            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void Unregister(int id)
        {
            var notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;
            var notification = notifications.Where(x => x.UserInfo.ContainsKey(NSObject.FromObject(NotificationKey)) && x.UserInfo[NotificationKey].Equals(NSObject.FromObject(id)))
                .FirstOrDefault();
            //UIApplication.SharedApplication.CancelAllLocalNotifications();
            if (notification != null)
            {
                UIApplication.SharedApplication.CancelLocalNotification(notification);
                //UIApplication.SharedApplication.CancelAllLocalNotifications();
            }
        }
    }

    public static class DateTimeExtensions
    {
        private static DateTime nsUtcRef = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static double SecondsSinceNSRefenceDate(this DateTime dt)
        {
            return (dt.ToUniversalTime() - nsUtcRef).TotalSeconds;
        }

        public static NSDate ToNSDate(this DateTime dt)
        {
            return NSDate.FromTimeIntervalSinceReferenceDate(dt.SecondsSinceNSRefenceDate());
        }
    }
}
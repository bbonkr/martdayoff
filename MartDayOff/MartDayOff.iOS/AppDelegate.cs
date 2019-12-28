using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Foundation;
using MartDayOff.iOS.Services;
using MartDayOff.Services;
using UIKit;

namespace MartDayOff.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

            var containerStartup = new ContainerStartup_iOS();

            LoadApplication(new App(containerStartup));

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | 
                    UIUserNotificationType.Badge | 
                    UIUserNotificationType.Sound, 
                    null);

                app.RegisterUserNotificationSettings(notificationSettings);
            }

            if(options != null)
            {
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    UILocalNotification localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                   if(localNotification != null)
                    {
                        var alertView = new UIAlertView(localNotification.AlertAction, localNotification.AlertBody, null, "Ok", null);
                        alertView.Show();

                        // Reset app icon badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }

            return base.FinishedLaunching(app, options);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            base.ReceivedLocalNotification(application, notification);
            
            
            UIAlertController alertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            var localNotificationService=Xamarin.Forms.DependencyService.Get<ILocalNotificationService>();

            if (notification.UserInfo.ContainsKey(NSObject.FromObject(LocalNotificationService.NotificationKey)))
            {
                try
                {
                    var idObject = notification.UserInfo[NSObject.FromObject(LocalNotificationService.NotificationKey)];
                    var id = ((NSNumber)idObject).Int32Value;
                    var eventArgs = new LocalNotificationReceivedEventArgs(id);
                    localNotificationService.OnReceived(eventArgs);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            
        }
    }
}

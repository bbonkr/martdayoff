using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MartDayOff.Services;
using MartDayOff.Views;
using MartDayOff.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Autofac;
using kr.bbon.Xamarin.Forms;

namespace MartDayOff
{
    public partial class App : Application
    {
        public App(ContainerStartup containerStartup)
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();

            containerStartup.Build();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            //var notifyAt=DateTime.Now.AddMinutes(1);
            //var notification = new LocalNotification {
            //    Title = "알림",
            //    Body = $"{notifyAt:yyyy-MM-dd HH:mm:ss} 알림 요청되었습니다.",
            //    Id = 1,
            //    NotifyTime = notifyAt,
            //};

            var notification = GetNextNotification();

            var notificationService = DependencyService.Get<ILocalNotificationService>();
            notificationService.Register(notification);
            notificationService.Received += async (s, e) => {
                Debug.WriteLine("알림 받음");

                var localNotificationService = s as ILocalNotificationService;
                if(localNotificationService != null)
                {
                    await Task.Delay(200);
                    //localNotificationService.Unregister(e.Id);

                    var nextNotification = GetNextNotification();

                    localNotificationService.Register(nextNotification);
                }                
            };

            notificationService.Registered += (s, e) => {
                Debug.WriteLine("등록됨");
            };
            notificationService.Unregistered += (s, e) => {
                Debug.WriteLine("해제됨");
                Xamarin.Essentials.Preferences.Remove(PreferenceKeys.NotificationId);
            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private LocalNotification GetNextNotification()
        {
            
            Random generator = new Random();
            var randomNumber = generator.Next(100000, 999999);

            var id = Xamarin.Essentials.Preferences.Get(PreferenceKeys.NotificationId, randomNumber);

            var seconds = generator.Next(20, 40);

            Xamarin.Essentials.Preferences.Set(PreferenceKeys.NotificationId, id);

            var notifyAt = DateTime.Now.AddSeconds(seconds);
            var notification = new LocalNotification
            {
                Id = id,
                Title = "알림",
                Body = $"{notifyAt:yyyy-MM-dd HH:mm:ss} 알림 요청되었습니다.${Environment.NewLine}등록시각{DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                NotifyTime = notifyAt,
            };

            return notification;
        }
    }
}

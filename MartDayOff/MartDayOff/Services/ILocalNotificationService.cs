using System;
using MartDayOff.Models;

namespace MartDayOff.Services
{
    public interface ILocalNotificationService
    {
        event EventHandler<LocalNotificationReceivedEventArgs> Received;
        event EventHandler<LocalNotificationRegisteredEventArgs> Registered;
        event EventHandler<LocalNotificationUnregisteredEventArgs> Unregistered;

        void Register(LocalNotification localNotification);

        void Unregister(int id);

        void OnReceived(LocalNotificationReceivedEventArgs e);
        void OnRegistered(LocalNotificationRegisteredEventArgs e);
        void OnUnregistered(LocalNotificationUnregisteredEventArgs e);
    }
}

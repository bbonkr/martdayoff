using System;
namespace MartDayOff
{
    public class LocalNotificationRegisteredEventArgs : EventArgs
    {
        public LocalNotificationRegisteredEventArgs(int id)
            :base()
        {
            this.Id = id;
        }

        public int Id { get; private set; }
    }

    public class LocalNotificationUnregisteredEventArgs : EventArgs
    {
        public LocalNotificationUnregisteredEventArgs(int id): base()
        {
            this.Id = id;
        }

        public int Id { get; private set; }
    }

    public class LocalNotificationReceivedEventArgs: EventArgs
    {
        public LocalNotificationReceivedEventArgs(int id)
            : base()
        {
            this.Id = id;
        }

        public int Id { get; private set; }
    }
}

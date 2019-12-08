using System;
using Autofac;

namespace MartDayOff.Droid
{
    public class ContainerStartup_Android : ContainerStartup
    {
        public ContainerStartup_Android()
        {
        }

        protected override void Register(ContainerBuilder builder)
        {
            base.Register(builder);
        }
    }
}

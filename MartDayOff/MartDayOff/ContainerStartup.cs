using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Util;
using kr.bbon.Xamarin.Forms;
using MartDayOff.ViewModels;

namespace MartDayOff
{
    public class ContainerStartup : kr.bbon.Xamarin.Forms.AppContainerBuilder
    {
        protected override void Register(ContainerBuilder builder)
        {
            RegisterViewModel(builder);

            base.Register(builder);
        }

        private void RegisterViewModel(ContainerBuilder builder)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var viewModelTypes = assembly.GetLoadableTypes()
                .Where(t => t.IsAssignableTo<ViewModelBase>() && t != typeof(ViewModelBase));

            foreach (var viewModeltype in viewModelTypes)
            {
                builder.RegisterType(viewModeltype).AsSelf();
            }
        }
    }
}

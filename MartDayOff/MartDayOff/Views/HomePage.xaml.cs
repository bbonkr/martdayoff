using System;
using System.Collections.Generic;
using kr.bbon.Xamarin.Forms;
using MartDayOff.ViewModels;
using Xamarin.Forms;

namespace MartDayOff.Views
{
    [Route(Routes.Home)]
    public partial class HomePage : AppContentPage<HomeViewModel>
    {
        public HomePage()
            :base()
        {
            InitializeComponent();            
        }
    }
}

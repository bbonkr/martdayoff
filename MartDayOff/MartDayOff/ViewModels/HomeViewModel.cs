using System;
using System.Collections.Generic;
using kr.bbon.Xamarin.Forms;
using Xamarin.Forms;

namespace MartDayOff.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
            : base()
        {

        }

        

        public string BackgoundColor
        {
            get => backgroundColor;
            set => SetProperty(ref backgroundColor, value);
        }

        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        private void CheckDayOff()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                DateTime.Now
            }
        }

        private void Get2ndAnd4thSunday(DateTime basis)
        {
            var results = new List<DateTime>();
            var firstDateOfMonth = new DateTime(basis.Year, basis.Month, 1);
            var days =DateTime.DaysInMonth(basis.Year, basis.Month);

            var check = 0;

            for(int i = 0; i<days; i++)
            {
                var currentDate = firstDateOfMonth.AddDays(i);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    check += 1;
                }

                if(check == 2 || check == 4) {
                    results.Add(currentDate);
                }
            }
        }

        private string backgroundColor;
        private string statusText;
    }
}


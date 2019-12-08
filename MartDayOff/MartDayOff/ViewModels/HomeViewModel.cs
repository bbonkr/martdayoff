using System;
using System.Collections.Generic;
using System.Globalization;
using kr.bbon.Xamarin.Forms;
using Xamarin.Forms;

namespace MartDayOff.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public HomeViewModel()
            : base()
        {
            Title = "마트쉬는 날";
            initialize();
        }        

        public string BackgoundColor
        {
            get => backgroundColor;
            set => SetProperty(ref backgroundColor, value);
        }

        public string TextColor
        {
            get => textColor;
            set => SetProperty(ref textColor, value);
        }

        public string StatusText
        {
            get => statusText;
            set => SetProperty(ref statusText, value);
        }

        public string TodayText {
            get => todayText;
            set => SetProperty(ref todayText, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private void initialize()
        {
            var dayoff = CheckDayOff();

            CultureInfo cultures = CultureInfo.CreateSpecificCulture("ko-KR");
            TodayText =string.Format(cultures, "오늘은 {0:yyyy년 MM월 dd일 ddd요일}입니다.", DateTime.Now);

            BackgoundColor = dayoff ? "#aadd0000" : "#aa00dd00";
            TextColor = dayoff ? "#eedddddd" : "#eedddddd";
            StatusText = dayoff ? "휴점" : "개점";

            Description = "대형 마트는 월 2회 의무 휴업이 강제되고 있습니다. 보통 두번째 일요일, 네번째 일요일을 휴무일로 정해져 있습니다.";
        }

        private bool CheckDayOff()
        {
            var today = DateTime.Now.AddDays(1);
            var list = Get2ndAnd4thSunday(today);
            return list.Contains($"{today:yyyy-MM-dd}");
        }

        private List<string> Get2ndAnd4thSunday(DateTime basis)
        {
            var results = new List<string>();
            var firstDateOfMonth = new DateTime(basis.Year, basis.Month, 1);
            var days = DateTime.DaysInMonth(basis.Year, basis.Month);

            var check = 0;

            for (int i = 0; i < days; i++)
            {
                var currentDate = firstDateOfMonth.AddDays(i);
                if (currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    check += 1;
                }

                if (check == 2 || check == 4) {
                    results.Add($"{ currentDate:yyyy-MM-dd}");
                }
            }
            return results;
        }

        private string backgroundColor;
        private string textColor;
        private string description;
        private string statusText;
        private string todayText;
    }
}


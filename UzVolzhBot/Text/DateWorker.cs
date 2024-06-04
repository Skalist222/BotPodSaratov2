using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodrostkiBot.Text
{
    public class DateWorker
    {
        public static DateTime GetNextSunday()
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                if(now.DayOfWeek.ToString() == "Sunday") 
                    return now;
                now = now.AddDays(1);
            }
            return now;
        }
        public static DateTime GetPreviousSunday() 
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i < 7; i++)
            {
                string dayName = now.DayOfWeek.ToString();
                if (dayName == "Sunday") return now;
                now = now.AddDays(-1);
            }
            return now;
        }
        public static DateTime[] GetNextThreeSundays()
        {
            DateTime nextSunday = GetNextSunday();
            return new DateTime[3]{nextSunday,nextSunday.AddDays(7),nextSunday.AddDays(14)};
        }
    }
}

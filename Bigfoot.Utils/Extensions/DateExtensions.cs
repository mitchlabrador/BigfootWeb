using System;
using System.Data.SqlTypes;
using System.Linq;

namespace Bigfoot.Utils.Extensions
{
    public static class DateExtensions
    {

        public static string ToSafeShortDate(this DateTime d)
        {
            return d.HasValue() ? d.ToShortDateString() : "";
        }

        
        public static DateTime DayStart(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, 0);
        }

        public static string ToJSShortDateString(this DateTime d)
        {
            return d.ToString("yyyy/MM/dd");
        }

        public static DateTime DayEnd(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 23, 59, 59, 997);
        }

        public static bool HasValue(this DateTime d)
        {
            return (d > DateTime.MinValue && d < DateTime.MaxValue) &&
                   (d > SqlDateTime.MinValue.Value && d < SqlDateTime.MaxValue.Value);
        }

        public static bool IsGreaterThan(this DateTime d, DateTime compareTo)
        {
            if (!d.HasValue()) return false;
            if (!compareTo.HasValue()) return true;
            return d > compareTo;
        }

        public static bool IsLessThan(this DateTime d, DateTime compareTo)
        {
            if (!d.HasValue()) return false;
            if (!compareTo.HasValue()) return true;
            return d < compareTo;
        }

        public static bool IsBetween(this DateTime d, DateTime start, DateTime end)
        {
            return d >= start.DayStart() && d <= end.DayEnd();
        }

        public static int DaysSince(this DateTime d, DateTime since)
        {
            int days = 0;
            if (d.HasValue() && since.HasValue()) 
            {
                days = int.Parse(Math.Ceiling(d.DayEnd().Subtract(since.DayStart()).TotalDays).ToString());
            }
            return days;
        }
        


        public static string ToShortDateTimeWithMilliseconds(this DateTime dt)
        {
            return dt.Year + "-" + dt.Month + "-" + dt.Day + " " +
                    dt.Hour + ":" + dt.Minute + ":" + dt.Second + "." + dt.Millisecond;
        }

        public static string ToFriendlyDate(this DateTime dt)
        {
            if (dt.DayStart() < DateTime.Now.DayStart())
                return dt.ToShortDateString() + " " + dt.ToShortTimeString();
            else
                return "Today @ " + dt.ToShortTimeString();
        }

        public static string ToFriendlyTimeAgoDate(this DateTime dt)
        {
            TimeSpan span = DateTime.Now.Subtract(dt);
            if (span.Days >= 365)
            {
                return string.Format("{0} ago", ((span.Days / 365) == 1 ? "a year" : (span.Days / 365).ToString() + " years"));
            }
            else if (span.Days >= 30)
            {
                return string.Format("{0} ago", ((span.Days / 30) == 1 ? "a month" : (span.Days / 30).ToString() + " months"));
            }
            else if (span.Days >= 1)
            {
                return string.Format("{0} ago", (span.Days == 1 ? "a day" : span.Days.ToString() + " days"));
            }
            else if (span.Hours >= 1)
            {
                return string.Format("{0} ago", (span.Hours == 1 ? "an hour" : span.Hours.ToString() + " hours"));
            }
            else if (span.Minutes >= 1)
            {
                return string.Format("{0} ago", (span.Minutes == 1 ? "a minute" : span.Minutes.ToString() + " minutes"));
            }
            else
            {
                return string.Format("{0} ago", (span.Seconds <= 1 ? "a moment" : span.Seconds.ToString() + " seconds"));
            }
        }

        public static string ToStringDisplay(this DateTime d)
        {
            return ToStringDisplay(d);
        }

        public static string ToStringDisplay(this DateTime d, string format)
        {
            if (d == DateTime.MinValue)
                return string.Empty;
            else if (format.IsNullOrEmpty())
                return d.ToString();
            else
                return d.ToString(format);
        }

        public static bool IsEmpty(this DateTime d)
        {
            return (d == DateTime.MinValue || d == null);
        }

        public static bool IsNotEmpty(this DateTime d)
        {
            return (d != DateTime.MinValue && d != null);
        }

    }
}
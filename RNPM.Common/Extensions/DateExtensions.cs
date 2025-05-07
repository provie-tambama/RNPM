using System;
using System.Globalization;

namespace RNPM.Common.Extensions;

public static class DateExtensions
{
    public static DateTime StartOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
    }

    public static DateTime EndOfDay(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
    }

    public static DateTimeOffset StartOfDay(this DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, date.Month, date.Day, 00, 00, 00, new TimeSpan());
    }

    public static DateTimeOffset EndOfDay(this DateTimeOffset date)
    {
        return new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, new TimeSpan());
    }

    public static double GetDateDifference(DateTime startDate, DateTime endDate)
    {
        return (endDate - startDate).TotalDays;
    }

    public static int MonthEndDay(this DateTime date, int lastDay)
    {
        var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        return lastDay > daysInMonth ? daysInMonth : lastDay;
    }

    public static int DaysInMonth(this DateTime date)
    {
        return DateTime.DaysInMonth(date.Year, date.Month);
    }

    public static DateTime MonthEndDate(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.DaysInMonth()).EndOfDay();
    }

    public static DateTime ToDate(this string date, string format)
    {
        return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
    }
}

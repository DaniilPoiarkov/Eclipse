#pragma warning disable IDE0130

namespace System;

public static class DateTimeExtensions
{
    /// <summary>
    /// <code>
    /// new TimeOnly(dateTime.Hour, dateTime.Minute);
    /// </code>
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static TimeOnly GetTime(this DateTime dateTime)
    {
        return new(dateTime.Hour, dateTime.Minute);
    }

    /// <summary>
    /// Returns the future day of the week or current date if it is the specified day of week and <a href="includeCurrentDaten"></a> is true.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="day">The day.</param>
    /// <param name="includeCurrentDate">if set to <c>true</c> [include current date].</param>
    /// <returns></returns>
    public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek day, bool includeCurrentDate = false)
    {
        return FindDayOfWeek(dateTime, day, 1, includeCurrentDate);
    }

    /// <summary>
    /// Returns the past day of the week or current date if it is the specified day of week and <a href="includeCurrentDaten"></a> is true.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="day">The day.</param>
    /// <param name="includeCurrentDate">if set to <c>true</c> [include current date].</param>
    /// <returns></returns>
    public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek day, bool includeCurrentDate = false)
    {
        return FindDayOfWeek(dateTime, day, -1, includeCurrentDate);
    }

    private static DateTime FindDayOfWeek(this DateTime dateTime, DayOfWeek day, int increment, bool includeCurrentDate)
    {
        if (!includeCurrentDate)
        {
            dateTime = dateTime.AddDays(increment);
        }

        while (dateTime.DayOfWeek != day)
        {
            dateTime = dateTime.AddDays(increment);
        }

        return dateTime;
    }

    /// <summary>
    /// Returns current date with specified time.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="hour">The hour.</param>
    /// <param name="minute">The minute.</param>
    /// <returns></returns>
    public static DateTime WithTime(this DateTime dateTime, int hour, int minute)
    {
        return dateTime.WithTime(new TimeOnly(hour, minute));
    }

    /// <summary>
    /// Returns current date with specified time.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="timeOnly">The time only.</param>
    /// <returns></returns>
    public static DateTime WithTime(this DateTime dateTime, TimeOnly timeOnly)
    {
        return new DateTime(
            new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
            timeOnly
        );
    }

    /// <summary>
    /// Returns the first day of the next month.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static DateTime NextMonth(this DateTime dateTime)
    {
        dateTime = dateTime.AddMonths(1);
        return new DateTime(dateTime.Year, dateTime.Month, 1)
            .WithTime(0, 0);
    }
}

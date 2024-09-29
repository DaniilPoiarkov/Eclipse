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

    private static DateTime FindDayOfWeek(this DateTime dateTime, DayOfWeek day, int increment, bool includeCurrentDate = false)
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

    public static DateTime WithTime(this DateTime dateTime, int hour, int minute)
    {
        return dateTime.WithTime(new TimeOnly(hour, minute));
    }

    public static DateTime WithTime(this DateTime dateTime, TimeOnly timeOnly)
    {
        return new DateTime(
            new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day),
            timeOnly
        );
    }
}

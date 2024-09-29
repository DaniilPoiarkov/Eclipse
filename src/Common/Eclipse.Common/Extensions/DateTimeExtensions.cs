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
    /// Returns the future day of the week or current date if it is the specified day of week
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek day)
    {
        return FindDayOfWeek(dateTime, day, 1);
    }

    /// <summary>
    /// Returns the past day of the week or current date if it is the specified day of week
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="day">The day.</param>
    /// <returns></returns>
    public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek day)
    {
        return FindDayOfWeek(dateTime, day, -1);
    }

    private static DateTime FindDayOfWeek(this DateTime dateTime, DayOfWeek day, int increment)
    {
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

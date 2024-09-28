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
    /// Returns the future day of the week
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="day"></param>
    /// <returns></returns>
    public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek day)
    {
        while (dateTime.DayOfWeek != day)
        {
            dateTime = dateTime.AddDays(1);
        }

        return dateTime;
    }
}

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
}

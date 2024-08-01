namespace Eclipse.Common.Clock;

public static class Clock
{
    private static readonly object _lock = new();

    public static ITimeProvider Provider { get; set; } = new UtcNowTimeProvider();

    public static DateTime Now
    {
        get
        {
            lock (_lock)
            {
                return Provider.Now;
            }
        }
    }
}

namespace Eclipse.Common.Clock;

public static class Clock
{
    public static readonly ITimeProvider Provider = new UtcNowTimeProvider();
    
    public static DateTime Now => Provider.Now;
}

namespace Eclipse.Application.Quartz.JobConfigurations;

internal class TestJobConfigurationOptions
{
    public long ChatId { get; }

    public TimeZoneInfo TimeZone { get; }

    public int Hours { get; }

    public int Minutes { get; }

    public TestJobConfigurationOptions(long chatId, TimeZoneInfo timeZone, int hours, int minutes)
    {
        ChatId = chatId;
        TimeZone = timeZone;
        Hours = hours;
        Minutes = minutes;
    }
}

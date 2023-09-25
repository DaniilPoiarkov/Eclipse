using Eclipse.Infrastructure.Quartz;

using Quartz;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJobConfiguration : IJobConfiguration
{
    public IJobDetail BuildJob()
    {
        return JobBuilder.Create<MorningJob>()
            .Build();
    }

    public ITrigger BuildTrigger()
    {
        var tzi = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        return TriggerBuilder.Create()
            .WithSchedule(
                CronScheduleBuilder
                    .DailyAtHourAndMinute(9, 0)
                    .InTimeZone(tzi))
            .Build();
    }
}

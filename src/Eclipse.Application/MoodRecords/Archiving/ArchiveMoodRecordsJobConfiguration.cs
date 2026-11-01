using Eclipse.Application.OptionsConfigurations;

using Quartz;

namespace Eclipse.Application.MoodRecords.Archiving;

internal sealed class ArchiveMoodRecordsJobConfiguration : IJobConfiguration
{
    public void Schedule(IQuartzBuilder quartz)
    {
        var jobKey = new JobKey(nameof(ArchiveMoodRecordsJob), "mood-records");

        quartz.AddJob<ArchiveMoodRecordsJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithInterval(1, IntervalUnit.Month))
                .StartAt(
                    DateTime.UtcNow.NextMonth()
                        .WithTime(0, 0)
                    ));
    }
}

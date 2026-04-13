using Eclipse.Application.OptionsConfigurations;

using Quartz;

namespace Eclipse.Application.MoodRecords.Archiving;

internal sealed class ArchiveMoodRecordsJobConfiguration : IJobConfiguration
{
    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ArchiveMoodRecordsJob), "mood-records");

        options.AddJob<ArchiveMoodRecordsJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithIntervalInMonths(1))
                .StartAt(
                    DateTime.UtcNow.NextMonth()
                        .WithTime(0, 0)
                    ));
    }
}

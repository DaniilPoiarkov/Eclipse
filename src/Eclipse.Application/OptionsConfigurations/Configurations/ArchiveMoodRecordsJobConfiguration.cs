using Eclipse.Application.MoodRecords.Jobs;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ArchiveMoodRecordsJobConfiguration : IJobConfiguration
{
    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ArchiveMoodRecordsJob));

        options.AddJob<ArchiveMoodRecordsJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithIntervalInMonths(1))
                .StartAt(DateTime.UtcNow.NextMonth()));
    }
}

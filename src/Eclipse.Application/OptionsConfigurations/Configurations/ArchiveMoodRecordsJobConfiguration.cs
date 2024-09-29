using Eclipse.Application.MoodRecords.Jobs;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ArchiveMoodRecordsJobConfiguration : IJobConfiguration
{
    public void Schedule(QuartzOptions options)
    {
        // TODO: job must start ad 1st day of the month with monthly interval.

        var jobKey = JobKey.Create(nameof(ArchiveMoodRecordsJob));

        options.AddJob<ArchiveMoodRecordsJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithIntervalInWeeks(1))
                .StartAt(new DateTimeOffset(
                    DateTime.UtcNow
                        .NextDayOfWeek(DayOfWeek.Sunday)
                        .WithTime(new TimeOnly(23, 00)))
                )
            );
    }
}

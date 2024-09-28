using Eclipse.Application.MoodRecords.Jobs;
using Eclipse.Application.OutboxMessages.Jobs;
using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal sealed class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _processOutboxMessagesJobDelay = 15;

    public void Configure(QuartzOptions options)
    {
        AddArchiveMoodRecordsJob(options);
        AddProcessOutboxMessagesJob(options);
        AddDeleteSuccessfullyProcessedOutboxMessagesJob(options);
    }

    private static void AddArchiveMoodRecordsJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ArchiveMoodRecordsJob));

        options.AddJob<ArchiveMoodRecordsJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithIntervalInWeeks(1))
                .StartAt(new DateTimeOffset(
                    DateTime.UtcNow.NextDayOfWeek(DayOfWeek.Sunday))
                )
            );
    }

    private static void AddProcessOutboxMessagesJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ProcessOutboxMessagesJob));

        options.AddJob<ProcessOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(_processOutboxMessagesJobDelay)
                    .RepeatForever())
                .StartNow());
    }

    private static void AddDeleteSuccessfullyProcessedOutboxMessagesJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(DeleteSuccessfullyProcessedOutboxMessagesJob));

        options.AddJob<DeleteSuccessfullyProcessedOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .StartNow());
    }
}

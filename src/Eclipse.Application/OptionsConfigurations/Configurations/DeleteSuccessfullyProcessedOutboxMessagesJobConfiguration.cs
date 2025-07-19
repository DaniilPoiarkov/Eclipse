using Eclipse.Application.OutboxMessages;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 24;

    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(DeleteSuccessfullyProcessedOutboxMessagesJob));

        options.AddJob<DeleteSuccessfullyProcessedOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInHours(_delay)
                    .RepeatForever())
                .StartNow());
    }
}

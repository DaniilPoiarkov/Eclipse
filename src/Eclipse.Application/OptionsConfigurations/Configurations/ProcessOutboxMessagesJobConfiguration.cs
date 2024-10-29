using Eclipse.Application.OutboxMessages.Jobs;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ProcessOutboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _processOutboxMessagesJobDelay = 5;

    public void Schedule(QuartzOptions options)
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
}

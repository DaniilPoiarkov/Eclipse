using Eclipse.Application.Common;
using Quartz;

namespace Eclipse.Application.OutboxMessages.ProcessMessages;

internal class ProcessOutboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _intervalInSeconds = 5;

    public void Configure(IServiceCollectionQuartzConfigurator configurator)
    {
        var jobKey = JobKey.Create(nameof(ProcessOutboxMessagesJob));

        configurator.AddJob<ProcessOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(_intervalInSeconds)
                    .RepeatForever())
                .StartNow());
    }
}

using Eclipse.Application.Common;

using Quartz;

namespace Eclipse.Application.OutboxMessages.DeleteSuccessfullyProcessedMessages;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration : IJobConfiguration
{
    public void Configure(IServiceCollectionQuartzConfigurator configurator)
    {
        var jobKey = JobKey.Create(nameof(DeleteSuccessfullyProcessedOutboxMessagesJob));

        configurator.AddJob<DeleteSuccessfullyProcessedOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .StartNow());
    }
}

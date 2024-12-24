using Eclipse.Application.OutboxMessages;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration : IJobConfiguration
{
    public void Schedule(QuartzOptions options)
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

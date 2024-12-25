using Eclipse.Application.InboxMessages;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class DeleteProcessedInboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 24;

    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(DeleteProcessedInboxMessagesJob));

        options.AddJob<DeleteProcessedInboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInHours(_delay)
                    .RepeatForever())
                .StartNow());
    }
}

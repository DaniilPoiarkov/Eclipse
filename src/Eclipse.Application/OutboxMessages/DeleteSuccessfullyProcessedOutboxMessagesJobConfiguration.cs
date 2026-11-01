using Eclipse.Application.OptionsConfigurations;

using Quartz;

namespace Eclipse.Application.OutboxMessages;

internal sealed class DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 24;

    public void Schedule(IQuartzBuilder quartz)
    {
        var jobKey = new JobKey(nameof(DeleteSuccessfullyProcessedOutboxMessagesJob), "events-processing");

        quartz.AddJob<DeleteSuccessfullyProcessedOutboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromHours(_delay))
                    .RepeatForever())
                .StartNow());
    }
}

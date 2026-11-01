using Eclipse.Application.OptionsConfigurations;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class DeleteProcessedInboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 24;

    public void Schedule(IQuartzBuilder quartz)
    {
        var jobKey = new JobKey(nameof(DeleteProcessedInboxMessagesJob), "events-processing");

        quartz.AddJob<DeleteProcessedInboxMessagesJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
            .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromHours(_delay))
                    .RepeatForever())
                .StartNow());
    }
}

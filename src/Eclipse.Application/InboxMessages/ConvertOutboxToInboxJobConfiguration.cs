using Eclipse.Application.OptionsConfigurations;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class ConvertOutboxToInboxJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 5;

    public void Schedule(IQuartzBuilder quartz)
    {
        var jobKey = new JobKey(nameof(ConvertOutboxToInboxJob), "events-processing");

        quartz.AddJob<ConvertOutboxToInboxJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(_delay))
                    .RepeatForever())
                .StartNow());
    }
}

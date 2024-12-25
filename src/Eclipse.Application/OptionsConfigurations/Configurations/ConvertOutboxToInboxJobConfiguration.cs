using Eclipse.Application.InboxMessages;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ConvertOutboxToInboxJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 5;

    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(ConvertOutboxToInboxJob));

        options.AddJob<ConvertOutboxToInboxJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(_delay)
                    .RepeatForever())
                .StartNow());
    }
}

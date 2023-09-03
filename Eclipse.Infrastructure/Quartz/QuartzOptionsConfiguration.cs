using Eclipse.Infrastructure.Quartz.Jobs;
using Microsoft.Extensions.Options;
using Quartz;

namespace Eclipse.Infrastructure.Quartz;

internal class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(WarmupJob));

        options.AddJob<WarmupJob>(b => b.WithIdentity(jobKey))
            .AddTrigger(b => b.ForJob(jobKey)
                .StartNow()
                .WithSimpleSchedule(s =>
                    s.WithIntervalInHours(1)
                    .RepeatForever()));
    }
}

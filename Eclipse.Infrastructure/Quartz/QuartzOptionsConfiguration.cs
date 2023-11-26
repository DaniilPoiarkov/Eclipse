using Eclipse.Infrastructure.Jobs.HealthCheck;
using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Infrastructure.Quartz;

internal class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _minutesDelay = 30;

    public void Configure(QuartzOptions options)
    {
        AddHealthCheckJob(options);
    }

    private static void AddHealthCheckJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(HealthCheckJob));

        options.AddJob<HealthCheckJob>(b => b.WithIdentity(jobKey))
            .AddTrigger(b => b.ForJob(jobKey)
                .StartNow()
                .WithSimpleSchedule(s =>
                    s.WithIntervalInMinutes(_minutesDelay)
                    .RepeatForever()));
    }
}

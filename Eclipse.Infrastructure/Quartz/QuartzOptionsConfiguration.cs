using Eclipse.Infrastructure.Quartz.Jobs;
using Microsoft.Extensions.Options;
using Quartz;

namespace Eclipse.Infrastructure.Quartz;

internal class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _hoursDelay = 1;
    public void Configure(QuartzOptions options)
    {
        AddWarmupJob(options);
        AddBotHealthCheckJob(options);
    }

    private static void AddBotHealthCheckJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(BotHealthCheckJob));

        options.AddJob<BotHealthCheckJob>(b => b.WithIdentity(jobKey))
            .AddTrigger(b => b.ForJob(jobKey)
                .StartNow()
                .WithSimpleSchedule(s =>
                    s.WithIntervalInHours(_hoursDelay)
                        .RepeatForever()));
    }

    private static void AddWarmupJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(WarmupJob));

        options.AddJob<WarmupJob>(b => b.WithIdentity(jobKey))
            .AddTrigger(b => b.ForJob(jobKey)
                .StartNow()
                .WithSimpleSchedule(s =>
                    s.WithIntervalInHours(_hoursDelay)
                    .RepeatForever()));
    }
}

using Eclipse.Pipelines.Jobs.Evening;
using Eclipse.Pipelines.Jobs.Morning;
using Eclipse.Pipelines.Jobs.Reminders;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Pipelines.Configurations;

internal class QuatzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _oneMinuteScanInterval = 1;

    public void Configure(QuartzOptions options)
    {
        AddJobWithEveryMinuteFire<MorningJob>(options);
        AddJobWithEveryMinuteFire<SendRemindersJob>(options);
        AddJobWithEveryMinuteFire<EveningJob>(options);
    }

    private static void AddJobWithEveryMinuteFire<TJob>(QuartzOptions options)
        where TJob : IJob
    {
        var jobKey = JobKey.Create(typeof(TJob).Name);

        options.AddJob<TJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(
                trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(s => s.WithIntervalInMinutes(_oneMinuteScanInterval)
                    .RepeatForever())
                    .StartNow()
            );
    }
}

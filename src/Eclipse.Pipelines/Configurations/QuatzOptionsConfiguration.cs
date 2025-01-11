using Eclipse.Pipelines.Jobs.Evening;
using Eclipse.Pipelines.Jobs.Morning;
using Eclipse.Pipelines.Jobs.SendMoodReport;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Pipelines.Configurations;

internal sealed class QuatzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _oneMinuteScanInterval = 1;

    public void Configure(QuartzOptions options)
    {
        // TODO: Make those jobs configured per user.
        AddJobWithEveryMinuteFire<CollectMoodRecordsJob>(options);
        AddJobWithEveryMinuteFire<SendGoodMorningJob>(options);
        AddJobWithEveryMinuteFire<SendMoodReportJob>(options);
    }

    private static void AddJobWithEveryMinuteFire<TJob>(QuartzOptions options)
        where TJob : IJob
    {
        var jobKey = JobKey.Create(typeof(TJob).Name);

        options.AddJob<TJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(
                trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(_oneMinuteScanInterval)
                    .RepeatForever())
                    .StartNow()
            );
    }
}

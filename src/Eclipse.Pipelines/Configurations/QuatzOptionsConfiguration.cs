using Eclipse.Pipelines.Jobs.Evening;
using Eclipse.Pipelines.Jobs.Morning;
using Eclipse.Pipelines.Jobs.Reminders;
using Eclipse.Pipelines.Jobs.SendMoodReport;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Pipelines.Configurations;

internal sealed class QuatzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly int _oneMinuteScanInterval = 1;

    public void Configure(QuartzOptions options)
    {
        AddJobWithEveryMinuteFire<CollectMoodRecordsJob>(options);
        AddJobWithEveryMinuteFire<SendRemindersJob>(options);
        AddJobWithEveryMinuteFire<RemindToFinishTodoItemsJob>(options);
        AddJobWithEveryMinuteFire<SendGoodMorningJob>(options);
        AddMoodReportJob(options);
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

    private static void AddMoodReportJob(QuartzOptions options)
    {
        var key = JobKey.Create(nameof(SendMoodReportJob));

        options.AddJob<SendMoodReportJob>(job => job.WithIdentity(key))
            .AddTrigger(trigger => trigger.ForJob(key)
                .WithCalendarIntervalSchedule(schedule =>
                    schedule.WithIntervalInMinutes(_oneMinuteScanInterval))
                .StartAt(
                    DateTime.UtcNow
                        .NextDayOfWeek(DayOfWeek.Sunday, true)
                        .WithTime(0, 0)
                ));
    }
}

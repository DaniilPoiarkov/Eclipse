using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.CollectMoodRecords;

internal sealed class CollectMoodRecordScheduler : IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public CollectMoodRecordScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, CollectMoodRecordSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(CollectMoodRecordJob)}-{options.UserId}");

        var job = JobBuilder.Create<CollectMoodRecordJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new CollectMoodRecordJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(RemindersConsts.Evening7PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInHours(RemindersConsts.OneDayInHours)
                .RepeatForever()
            )
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, CollectMoodRecordSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(CollectMoodRecordJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}

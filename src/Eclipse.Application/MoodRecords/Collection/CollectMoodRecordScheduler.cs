using Eclipse.Application.Reminders;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed class CollectMoodRecordScheduler : INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>
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

        var time = _timeProvider.Now.WithTime(NotificationConsts.Evening7PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInHours(NotificationConsts.OneDayInHours)
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

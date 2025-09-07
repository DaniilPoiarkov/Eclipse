using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection;

internal sealed class CollectFeedbackScheduler : INotificationScheduler<CollectFeedbackJob, CollectFeedbackSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public CollectFeedbackScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task Schedule(IScheduler scheduler, CollectFeedbackSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var jobKey = JobKey.Create($"{nameof(CollectFeedbackJob)}-{options.UserId}");
        
        var job = JobBuilder.Create<CollectFeedbackJob>()
            .WithIdentity(jobKey)
            .UsingJobData("data", JsonConvert.SerializeObject(new CollectFeedbackJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.NextDayOfWeek(DayOfWeek.Monday)
            .WithTime(NotificationConsts.Day130PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithCalendarIntervalSchedule(schedule => 
                schedule.WithIntervalInWeeks(NotificationConsts.OneUnit)
            )
            .StartAt(time)
            .Build();

        return scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, CollectFeedbackSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var jobKey = JobKey.Create($"{nameof(CollectFeedbackJob)}-{options.UserId}");
        return scheduler.DeleteJob(jobKey, cancellationToken);
    }
}

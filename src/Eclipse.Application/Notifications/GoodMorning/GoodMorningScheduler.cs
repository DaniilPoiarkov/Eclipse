using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning;

internal sealed class GoodMorningScheduler : INotificationScheduler<GoodMorningJob, SchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public GoodMorningScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");

        var job = JobBuilder.Create<GoodMorningJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(NotificationConsts.Morning9AM)
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

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}

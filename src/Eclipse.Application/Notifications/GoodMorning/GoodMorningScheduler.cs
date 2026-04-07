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
        var job = await scheduler.GetJob<GoodMorningJob>(cancellationToken);

        var time = _timeProvider.Now.WithTime(NotificationConsts.Morning9AM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity(new TriggerKey($"{nameof(GoodMorningJob)}-{options.UserId}", options.UserId.ToString()))
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
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
        return scheduler.UnscheduleJob(
            new TriggerKey($"{nameof(GoodMorningJob)}-{options.UserId}", options.UserId.ToString()),
            cancellationToken
        );
    }
}

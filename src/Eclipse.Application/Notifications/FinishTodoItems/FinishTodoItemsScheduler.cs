using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems;

internal sealed class FinishTodoItemsScheduler : INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public FinishTodoItemsScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, FinishTodoItemsSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(FinishTodoItemsJob)}-{options.UserId}");

        var job = JobBuilder.Create<FinishTodoItemsJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new FinishTodoItemsJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(NotificationConsts.Evening6PM)
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

    public Task Unschedule(IScheduler scheduler, FinishTodoItemsSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(FinishTodoItemsJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}

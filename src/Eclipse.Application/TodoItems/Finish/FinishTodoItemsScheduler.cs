using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.TodoItems.Finish;

internal sealed class FinishTodoItemsScheduler : INotificationScheduler<FinishTodoItemsJob, SchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public FinishTodoItemsScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var job = await scheduler.GetOrAddDurableJob<FinishTodoItemsJob>(cancellationToken);

        var time = _timeProvider.Now.WithTime(NotificationConsts.Evening6PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity(new TriggerKey($"{nameof(FinishTodoItemsJob)}-{options.UserId}", options.UserId.ToString()))
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInHours(NotificationConsts.OneDayInHours)
                .RepeatForever()
            )
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        return scheduler.UnscheduleJob(
            new TriggerKey($"{nameof(FinishTodoItemsJob)}-{options.UserId}", options.UserId.ToString()),
            cancellationToken
        );
    }
}

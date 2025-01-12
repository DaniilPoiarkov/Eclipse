using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class RescheduleForNewTimeRemindToFinishTodoItemsHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    public RescheduleForNewTimeRemindToFinishTodoItemsHandler(ISchedulerFactory schedulerFactory, ITimeProvider timeProvider)
    {
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var key = JobKey.Create($"{nameof(RemindToFinishTodoItemsJob)}-{@event.UserId}");

        await scheduler.DeleteJob(key, cancellationToken);

        var job = JobBuilder.Create<RemindToFinishTodoItemsJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(@event.UserId)))
            .Build();

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInHours(RemindersConsts.OneDayInHours)
                .RepeatForever()
            )
            .StartAt(_timeProvider.Now.WithTime(RemindersConsts.Evening6PM).Add(@event.Gmt))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

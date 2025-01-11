using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class RescheduleForNewTimeRemindToFinishTodoItemsHandler : IEventHandler<GmtChangedDomainEvent>
{
    private static readonly TimeOnly _evening = new(18, 0);

    private static readonly int _interval = 24;

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
                .WithIntervalInHours(_interval)
                .RepeatForever()
            )
            .StartAt(_timeProvider.Now.Add(@event.Gmt).WithTime(_evening))
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

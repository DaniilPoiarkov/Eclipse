using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class ScheduleNewTimeFinishTodoItemsHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    public ScheduleNewTimeFinishTodoItemsHandler(
        ISchedulerFactory schedulerFactory,
        IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler();

        var key = JobKey.Create($"{nameof(FinishTodoItemsJob)}-{@event.UserId}");

        await scheduler.DeleteJob(key, cancellationToken);

        var options = new FinishTodoItemsSchedulerOptions(@event.UserId, @event.Gmt);

        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

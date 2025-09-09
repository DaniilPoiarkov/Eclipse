using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class NewTimeEventHandler<TJob> : IEventHandler<GmtChangedDomainEvent>
    where TJob : IJob
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<TJob, SchedulerOptions> _jobScheduler;

    public NewTimeEventHandler(ISchedulerFactory schedulerFactory, INotificationScheduler<TJob, SchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var options = new SchedulerOptions(@event.UserId, @event.Gmt);

        await _jobScheduler.Unschedule(scheduler, options, cancellationToken);
        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

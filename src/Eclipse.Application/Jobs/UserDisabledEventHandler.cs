using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class UserDisabledEventHandler<TJob> : IEventHandler<UserDisabledDomainEvent>
    where TJob : IJob
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<TJob, SchedulerOptions> _jobScheduler;

    public UserDisabledEventHandler(ISchedulerFactory schedulerFactory, INotificationScheduler<TJob, SchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await _jobScheduler.Unschedule(scheduler, new SchedulerOptions(@event.UserId, default), cancellationToken);
    }
}

using Eclipse.Application.Jobs;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class UnscheduleGoodMorningHandler : IEventHandler<UserDisabledDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<GoodMorningJob, SchedulerOptions> _jobScheduler;

    public UnscheduleGoodMorningHandler(ISchedulerFactory schedulerFactory, INotificationScheduler<GoodMorningJob, SchedulerOptions> jobScheduler)
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

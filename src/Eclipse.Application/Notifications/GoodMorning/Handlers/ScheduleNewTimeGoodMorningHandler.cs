using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleNewTimeGoodMorningHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> _jobScheduler;

    public ScheduleNewTimeGoodMorningHandler(
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var options = new GoodMorningSchedulerOptions(@event.UserId, @event.Gmt);

        await _jobScheduler.Unschedule(scheduler, options, cancellationToken);
        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

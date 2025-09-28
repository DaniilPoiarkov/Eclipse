using Eclipse.Application.Jobs;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal sealed class ScheduleNewTimeCollectFeedbackHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<CollectFeedbackJob, SchedulerOptions> _jobScheduler;

    public ScheduleNewTimeCollectFeedbackHandler(
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, SchedulerOptions> jobScheduler)
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

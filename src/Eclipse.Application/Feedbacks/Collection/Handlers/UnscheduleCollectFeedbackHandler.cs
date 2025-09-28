using Eclipse.Application.Jobs;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal sealed class UnscheduleCollectFeedbackHandler : IEventHandler<UserDisabledDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<CollectFeedbackJob, SchedulerOptions> _jobScheduler;

    public UnscheduleCollectFeedbackHandler(
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, SchedulerOptions> jobScheduler)
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

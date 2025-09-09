using Eclipse.Application.Jobs;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.MoodRecords.Collection.Handlers;

internal sealed class UnscheduleCollectMoodRecordHandler : IEventHandler<UserDisabledDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<CollectMoodRecordJob, SchedulerOptions> _jobScheduler;

    public UnscheduleCollectMoodRecordHandler(
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectMoodRecordJob, SchedulerOptions> jobScheduler)
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

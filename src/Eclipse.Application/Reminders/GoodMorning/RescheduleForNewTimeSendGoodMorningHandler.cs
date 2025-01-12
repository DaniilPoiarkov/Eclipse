using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class RescheduleForNewTimeSendGoodMorningHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<SendGoodMorningJob, SendGoodMorningSchedulerOptions> _jobScheduler;

    public RescheduleForNewTimeSendGoodMorningHandler(
        ISchedulerFactory schedulerFactory,
        IJobScheduler<SendGoodMorningJob, SendGoodMorningSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(SendGoodMorningJob)}-{@event.UserId}");

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.DeleteJob(key, cancellationToken);

        await _jobScheduler.Schedule(scheduler, new SendGoodMorningSchedulerOptions(@event.UserId, @event.Gmt), cancellationToken);
    }
}

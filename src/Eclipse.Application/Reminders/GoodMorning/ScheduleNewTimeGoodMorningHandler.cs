using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class ScheduleNewTimeGoodMorningHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> _jobScheduler;

    public ScheduleNewTimeGoodMorningHandler(
        ISchedulerFactory schedulerFactory,
        IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{@event.UserId}");

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.DeleteJob(key, cancellationToken);

        await _jobScheduler.Schedule(scheduler, new GoodMorningSchedulerOptions(@event.UserId, @event.Gmt), cancellationToken);
    }
}

using Eclipse.Application.Jobs;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal sealed class ScheduleNewTimeMoodReportHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<MoodReportJob, SchedulerOptions> _jobScheduler;

    public ScheduleNewTimeMoodReportHandler(
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<MoodReportJob, SchedulerOptions> jobScheduler)
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

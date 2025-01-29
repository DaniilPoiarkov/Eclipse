using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.Reminders.MoodReport;

internal sealed class ScheduleNewTimeMoodReportHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    public ScheduleNewTimeMoodReportHandler(
        ISchedulerFactory schedulerFactory,
        IJobScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var key = JobKey.Create($"{nameof(MoodReportJob)}-{@event.UserId}");

        await scheduler.DeleteJob(key, cancellationToken);

        var options = new MoodReportSchedulerOptions(@event.UserId, @event.Gmt);

        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

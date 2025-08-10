using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal sealed class UnscheduleMoodReportHandler : IEventHandler<UserDisabledDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    public UnscheduleMoodReportHandler(ISchedulerFactory schedulerFactory, INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await _jobScheduler.Unschedule(scheduler, new MoodReportSchedulerOptions(@event.UserId, default), cancellationToken);
    }
}

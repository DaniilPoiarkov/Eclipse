using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal sealed class ScheduleUserEnabledMoodReportHandler : MoodReportHandlerBase<UserEnabledDomainEvent>
{
    public ScheduleUserEnabledMoodReportHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleUserEnabledMoodReportHandler> logger,
        INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler) : base(userRepository, schedulerFactory, logger, jobScheduler)
    {
    }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

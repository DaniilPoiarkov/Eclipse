using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal sealed class ScheduleNewUserMoodReportHandler : MoodReportHandlerBase<NewUserJoinedDomainEvent>
{
    public ScheduleNewUserMoodReportHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleNewUserMoodReportHandler> logger,
        INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler) : base(userRepository, schedulerFactory, logger, jobScheduler)
    {

    }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal sealed class ScheduleUserEnabledMoodReportHandler : UserEventHandlerBase<UserEnabledDomainEvent, MoodReportJob, SchedulerOptions>
{
    public ScheduleUserEnabledMoodReportHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleUserEnabledMoodReportHandler> logger,
        INotificationScheduler<MoodReportJob, SchedulerOptions> jobScheduler)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override SchedulerOptions GetOptions(User user)
    {
        return new SchedulerOptions(user.Id, user.Gmt);
    }
}

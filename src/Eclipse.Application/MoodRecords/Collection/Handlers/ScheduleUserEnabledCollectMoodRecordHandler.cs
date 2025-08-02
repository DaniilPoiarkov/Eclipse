using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Collection.Handlers;

internal sealed class ScheduleUserEnabledCollectMoodRecordHandler : CollectMoodRecordHandlerBase<UserEnabledDomainEvent>
{
    public ScheduleUserEnabledCollectMoodRecordHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> jobScheduler,
        ILogger<ScheduleNewUserCollectMoodRecordHandler> logger) : base(userRepository, schedulerFactory, jobScheduler, logger)
    {
    }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

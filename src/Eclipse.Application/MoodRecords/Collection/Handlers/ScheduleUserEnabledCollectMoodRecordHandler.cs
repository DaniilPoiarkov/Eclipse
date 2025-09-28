using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Collection.Handlers;

internal sealed class ScheduleUserEnabledCollectMoodRecordHandler : UserEventHandlerBase<UserEnabledDomainEvent, CollectMoodRecordJob, SchedulerOptions>
{
    public ScheduleUserEnabledCollectMoodRecordHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectMoodRecordJob, SchedulerOptions> jobScheduler,
        ILogger<ScheduleUserEnabledCollectMoodRecordHandler> logger)
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

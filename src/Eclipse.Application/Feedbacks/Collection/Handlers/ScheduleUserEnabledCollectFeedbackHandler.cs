using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal sealed class ScheduleUserEnabledCollectFeedbackHandler : CollectFeedbackHandlerBase<UserEnabledDomainEvent>
{
    public ScheduleUserEnabledCollectFeedbackHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, CollectFeedbackSchedulerOptions> jobScheduler,
        ILogger logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

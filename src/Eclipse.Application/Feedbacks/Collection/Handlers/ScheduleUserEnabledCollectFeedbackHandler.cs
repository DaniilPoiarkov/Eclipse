using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal sealed class ScheduleUserEnabledCollectFeedbackHandler : UserEventHandlerBase<UserEnabledDomainEvent, CollectFeedbackJob, CollectFeedbackSchedulerOptions>
{
    public ScheduleUserEnabledCollectFeedbackHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, CollectFeedbackSchedulerOptions> jobScheduler,
        ILogger<ScheduleUserEnabledCollectFeedbackHandler> logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override CollectFeedbackSchedulerOptions GetOptions(User user)
    {
        return new CollectFeedbackSchedulerOptions(user.Id, user.Gmt);
    }
}

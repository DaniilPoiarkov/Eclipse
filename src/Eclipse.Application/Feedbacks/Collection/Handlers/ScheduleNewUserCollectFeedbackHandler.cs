using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal sealed class ScheduleNewUserCollectFeedbackHandler : UserEventHandlerBase<NewUserJoinedDomainEvent, CollectFeedbackJob, SchedulerOptions>
{
    public ScheduleNewUserCollectFeedbackHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, SchedulerOptions> jobScheduler,
        ILogger<ScheduleNewUserCollectFeedbackHandler> logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override SchedulerOptions GetOptions(User user)
    {
        return new SchedulerOptions(user.Id, user.Gmt);
    }
}

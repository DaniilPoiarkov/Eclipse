using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal sealed class ScheduleUserEnabledFinishTodoItemsHandler : UserEventHandlerBase<UserEnabledDomainEvent, FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>
{
    public ScheduleUserEnabledFinishTodoItemsHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler,
        ILogger<ScheduleUserEnabledFinishTodoItemsHandler> logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override FinishTodoItemsSchedulerOptions GetOptions(User user)
    {
        return new FinishTodoItemsSchedulerOptions(user.Id, user.Gmt);
    }
}

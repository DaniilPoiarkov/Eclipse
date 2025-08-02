using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal sealed class ScheduleUserEnabledFinishTodoItemsHandler : FinishTodoItemsHandlerBase<UserEnabledDomainEvent>
{
    public ScheduleUserEnabledFinishTodoItemsHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler,
        ILogger<ScheduleUserEnabledFinishTodoItemsHandler> logger) : base(userRepository, schedulerFactory, jobScheduler, logger)
    {

    }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

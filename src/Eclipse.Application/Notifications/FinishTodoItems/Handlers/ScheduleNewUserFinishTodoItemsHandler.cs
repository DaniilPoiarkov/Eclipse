using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal sealed class ScheduleNewUserFinishTodoItemsHandler : FinishTodoItemsHandlerBase<NewUserJoinedDomainEvent>
{
    public ScheduleNewUserFinishTodoItemsHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler,
        ILogger<ScheduleNewUserFinishTodoItemsHandler> logger) : base(userRepository, schedulerFactory, jobScheduler, logger)
    {
        
    }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

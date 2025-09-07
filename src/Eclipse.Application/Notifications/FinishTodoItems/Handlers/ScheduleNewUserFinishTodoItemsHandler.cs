using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal sealed class ScheduleNewUserFinishTodoItemsHandler : UserEventHandlerBase<NewUserJoinedDomainEvent, FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>
{
    public ScheduleNewUserFinishTodoItemsHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler,
        ILogger<ScheduleNewUserFinishTodoItemsHandler> logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override FinishTodoItemsSchedulerOptions GetOptions(User user)
    {
        return new FinishTodoItemsSchedulerOptions(user.Id, user.Gmt);
    }
}

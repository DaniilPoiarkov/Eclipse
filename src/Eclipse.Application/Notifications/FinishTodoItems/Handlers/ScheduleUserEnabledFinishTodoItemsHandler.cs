using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal sealed class ScheduleUserEnabledFinishTodoItemsHandler : UserEventHandlerBase<UserEnabledDomainEvent, FinishTodoItemsJob>
{
    public ScheduleUserEnabledFinishTodoItemsHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, SchedulerOptions> jobScheduler,
        ILogger<ScheduleUserEnabledFinishTodoItemsHandler> logger)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

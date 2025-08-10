using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.FinishTodoItems.Handlers;

internal abstract class FinishTodoItemsHandlerBase<TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    protected readonly IUserRepository UserRepository;

    protected readonly ISchedulerFactory SchedulerFactory;

    protected readonly INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> JobScheduler;

    protected readonly ILogger Logger;

    public FinishTodoItemsHandlerBase(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler,
        ILogger logger)
    {
        UserRepository = userRepository;
        SchedulerFactory = schedulerFactory;
        JobScheduler = jobScheduler;
        Logger = logger;
    }

    public abstract Task Handle(TEvent @event, CancellationToken cancellationToken = default);

    protected async Task Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await UserRepository.FindAsync(userId, cancellationToken);

        if (user is not { IsEnabled: true })
        {
            Logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(ScheduleNewUserFinishTodoItemsHandler), userId, "User not found or disabled.");
            return;
        }

        var scheduler = await SchedulerFactory.GetScheduler();

        await JobScheduler.Schedule(scheduler, new FinishTodoItemsSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

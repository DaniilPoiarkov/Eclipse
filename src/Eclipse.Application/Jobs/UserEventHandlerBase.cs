using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Jobs;

internal abstract class UserEventHandlerBase<TEvent, TJob> : IEventHandler<TEvent>
    where TJob : IJob
    where TEvent : IDomainEvent
{
    protected IUserRepository UserRepository { get; }

    protected ISchedulerFactory SchedulerFactory { get; }

    protected INotificationScheduler<TJob, SchedulerOptions> JobScheduler { get; }

    protected ILogger Logger { get; }

    protected UserEventHandlerBase(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<TJob, SchedulerOptions> jobScheduler,
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
            Logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}",
                typeof(TJob).Name,
                userId,
                user is null
                    ? "User not found."
                    : "User is disabled."
            );

            return;
        }

        var scheduler = await SchedulerFactory.GetScheduler(cancellationToken);
        await JobScheduler.Schedule(scheduler, new SchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Feedbacks.Collection.Handlers;

internal abstract class CollectFeedbackHandlerBase<TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    protected IUserRepository UserRepository { get; }

    protected ISchedulerFactory SchedulerFactory { get; }

    protected INotificationScheduler<CollectFeedbackJob, CollectFeedbackSchedulerOptions> JobScheduler { get; }

    protected ILogger Logger { get; }

    public CollectFeedbackHandlerBase(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<CollectFeedbackJob, CollectFeedbackSchedulerOptions> jobScheduler,
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
            Logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(CollectFeedbackJob), userId, "User not found or disabled.");
            return;
        }

        var scheduler = await SchedulerFactory.GetScheduler();
        await JobScheduler.Schedule(scheduler, new CollectFeedbackSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

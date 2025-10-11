using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class UserEventHandlerBase<TEvent, TJob> : IEventHandler<TEvent>
    where TJob : IJob
    where TEvent : IDomainEvent, IHasUserId
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<TJob, SchedulerOptions> _jobScheduler;

    private readonly ILogger<UserEventHandlerBase<TEvent, TJob>> _logger;

    public UserEventHandlerBase(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<TJob, SchedulerOptions> jobScheduler,
        ILogger<UserEventHandlerBase<TEvent, TJob>> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
        _logger = logger;
    }

    public Task Handle(TEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    private async Task Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is not { IsEnabled: true })
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}",
                typeof(TJob).Name,
                userId,
                user is null
                    ? "User not found."
                    : "User is disabled."
            );

            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        await _jobScheduler.Schedule(scheduler, new SchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

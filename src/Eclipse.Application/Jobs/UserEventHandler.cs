using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Shared.Users;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class UserEventHandler<TEvent, TJob> : IEventHandler<TEvent>
    where TJob : IJob
    where TEvent : IDomainEvent, IHasUserId
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<TJob, SchedulerOptions> _jobScheduler;

    private readonly ILogger<UserEventHandler<TEvent, TJob>> _logger;

    public UserEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<TJob, SchedulerOptions> jobScheduler,
        ILogger<UserEventHandler<TEvent, TJob>> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
        _logger = logger;
    }

    public async Task Handle(TEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is not { IsEnabled: true })
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}",
                typeof(TJob).Name,
                @event.UserId,
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

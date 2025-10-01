using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class UserEnabledEventHandler<TJob> : UserEventHandlerBase<UserEnabledDomainEvent, TJob>
    where TJob : IJob
{
    public UserEnabledEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<TJob, SchedulerOptions> jobScheduler,
        ILogger<UserEnabledEventHandler<TJob>> logger) : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

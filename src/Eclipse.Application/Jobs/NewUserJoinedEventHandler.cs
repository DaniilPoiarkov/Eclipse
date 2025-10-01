using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Jobs;

internal sealed class NewUserJoinedEventHandler<TJob> : UserEventHandlerBase<NewUserJoinedDomainEvent, TJob>
    where TJob : IJob
{
    public NewUserJoinedEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<TJob, SchedulerOptions> jobScheduler,
        ILogger<NewUserJoinedEventHandler<TJob>> logger) : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

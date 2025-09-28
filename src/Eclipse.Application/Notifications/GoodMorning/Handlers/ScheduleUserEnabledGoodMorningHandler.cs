using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleUserEnabledGoodMorningHandler : UserEventHandlerBase<UserEnabledDomainEvent, GoodMorningJob>
{
    public ScheduleUserEnabledGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleUserEnabledGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, SchedulerOptions> jobScheduler)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleUserEnabledGoodMorningHandler : GoodMorningHandlerBase<UserEnabledDomainEvent>
{
    public ScheduleUserEnabledGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleUserEnabledGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler) : base(userRepository, schedulerFactory, logger, jobScheduler)
    {

    }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

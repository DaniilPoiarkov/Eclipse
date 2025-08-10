using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleNewUserGoodMorningHandler : GoodMorningHandlerBase<NewUserJoinedDomainEvent>
{
    public ScheduleNewUserGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleNewUserGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler) : base(userRepository, schedulerFactory, logger, jobScheduler)
    {

    }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }
}

using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleUserEnabledGoodMorningHandler : UserEventHandlerBase<UserEnabledDomainEvent, GoodMorningJob, GoodMorningSchedulerOptions>
{
    public ScheduleUserEnabledGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleUserEnabledGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override GoodMorningSchedulerOptions GetOptions(User user)
    {
        return new GoodMorningSchedulerOptions(user.Id, user.Gmt);
    }
}

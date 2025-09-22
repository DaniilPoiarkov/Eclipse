using Eclipse.Application.Jobs;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Domain.Users.Handlers;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning.Handlers;

internal sealed class ScheduleNewUserGoodMorningHandler : UserEventHandlerBase<NewUserJoinedDomainEvent, GoodMorningJob, SchedulerOptions>
{
    public ScheduleNewUserGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleNewUserGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, SchedulerOptions> jobScheduler)
        : base(userRepository, schedulerFactory, jobScheduler, logger) { }

    public override Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    protected override SchedulerOptions GetOptions(User user)
    {
        return new SchedulerOptions(user.Id, user.Gmt);
    }
}

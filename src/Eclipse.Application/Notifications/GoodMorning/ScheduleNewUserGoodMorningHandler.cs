﻿using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Notifications.GoodMorning;

internal sealed class ScheduleNewUserGoodMorningHandler : IEventHandler<NewUserJoinedDomainEvent>, IEventHandler<UserEnabledDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<ScheduleNewUserGoodMorningHandler> _logger;

    private readonly INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> _jobScheduler;

    public ScheduleNewUserGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleNewUserGoodMorningHandler> logger,
        INotificationScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
        _jobScheduler = jobScheduler;
    }

    public Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    public Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        return Handle(@event.UserId, cancellationToken);
    }

    private async Task Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(ScheduleNewUserGoodMorningHandler), userId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler();

        await _jobScheduler.Schedule(scheduler, new GoodMorningSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

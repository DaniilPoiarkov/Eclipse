using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class ScheduleNewUserGoodMorningHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<ScheduleNewUserGoodMorningHandler> _logger;

    private readonly IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> _jobScheduler;

    public ScheduleNewUserGoodMorningHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleNewUserGoodMorningHandler> logger,
        IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(ScheduleNewUserGoodMorningHandler), @event.UserId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler();

        await _jobScheduler.Schedule(scheduler, new GoodMorningSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

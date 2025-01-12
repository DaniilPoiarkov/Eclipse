using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class ScheduleSendGoodMorningJobHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<ScheduleSendGoodMorningJobHandler> _logger;

    private readonly IJobScheduler<SendGoodMorningJob, SendGoodMorningSchedulerOptions> _jobScheduler;

    public ScheduleSendGoodMorningJobHandler(
        ISchedulerFactory schedulerFactory,
        IUserRepository userRepository,
        ILogger<ScheduleSendGoodMorningJobHandler> logger,
        IJobScheduler<SendGoodMorningJob, SendGoodMorningSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _userRepository = userRepository;
        _logger = logger;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(SendGoodMorningJob), @event.UserId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await _jobScheduler.Schedule(scheduler, new SendGoodMorningSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

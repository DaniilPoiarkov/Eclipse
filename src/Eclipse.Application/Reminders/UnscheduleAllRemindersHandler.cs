using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class UnscheduleAllRemindersHandler : IEventHandler<UserDisabledDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<UnscheduleAllRemindersHandler> _logger;

    public UnscheduleAllRemindersHandler(IUserRepository userRepository, ISchedulerFactory schedulerFactory, ILogger<UnscheduleAllRemindersHandler> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task Handle(UserDisabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Cannot unschedule reminders. User not found.");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var keys = user.Reminders
            .Select(r => JobKey.Create($"{nameof(SendReminderJob)}-{user.Id}-{r.Id}"))
            .ToArray();

        await scheduler.DeleteJobs(keys, cancellationToken);
    }
}

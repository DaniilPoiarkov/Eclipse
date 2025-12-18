using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Reminders;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderRescheduledEventHandler : IEventHandler<ReminderRescheduledDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<ReminderRescheduledEventHandler> _logger;

    public ReminderRescheduledEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider,
        ILogger<ReminderRescheduledEventHandler> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(ReminderRescheduledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var key = $"{nameof(SendReminderJob)}-{@event.UserId}-{@event.ReminderId}";
        var jobKey = new JobKey(key);

        var jobDetail = await scheduler.GetJobDetail(jobKey, cancellationToken);
        
        if (jobDetail is not null)
        {
            await scheduler.DeleteJob(jobKey, cancellationToken);
        }

        var time = _timeProvider.Now.WithTime(@event.NotifyAt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(new JobKey(key))
            .StartAt(time)
            .Build();

        await scheduler.RescheduleJob(new TriggerKey(key), trigger, cancellationToken);
    }
}

using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Reminders;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderRescheduledEventHandler : IEventHandler<ReminderRescheduledDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<ReminderRescheduledEventHandler> _logger;

    public ReminderRescheduledEventHandler(
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider,
        ILogger<ReminderRescheduledEventHandler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(ReminderRescheduledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            // =====
            var key = $"{nameof(SendReminderJob)}-{@event.UserId}-{@event.ReminderId}";

            var jobDetail = await scheduler.GetJobDetail(new JobKey(key), cancellationToken);
            var jobTrigger = await scheduler.GetTrigger(new TriggerKey(key), cancellationToken);
            // =====

            var time = _timeProvider.Now.WithTime(@event.NotifyAt);

            if (time < _timeProvider.Now)
            {
                time = time.NextDay();
            }

            var trigger = TriggerBuilder.Create()
                .ForJob(new JobKey($"{nameof(SendReminderJob)}-{@event.UserId}-{@event.ReminderId}"))
                .StartAt(time)
                .Build();

            await scheduler.ScheduleJob(trigger, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reschedule reminder {ReminderId} for user {UserId}.", @event.ReminderId, @event.UserId);
        }
    }
}

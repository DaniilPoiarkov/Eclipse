using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderRemovedEventHandler : IEventHandler<ReminderRemovedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<ReminderRemovedEventHandler> _logger;

    public ReminderRemovedEventHandler(ISchedulerFactory schedulerFactory, ILogger<ReminderRemovedEventHandler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
    }

    public async Task Handle(ReminderRemovedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        try
        {
            var removedSuccessfully = await scheduler.UnscheduleJob(
                new TriggerKey($"{nameof(SendReminderJob)}-{@event.UserId}-{@event.ReminderId}", @event.UserId.ToString()),
                cancellationToken
            );

            if (!removedSuccessfully)
            {
                _logger.LogWarning("Cannot unschedule {Job} after reminder was removed for user {UserId} and reminder {ReminderId}", nameof(SendReminderJob), @event.UserId, @event.ReminderId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot delete send reminder job after reminder was removed for user {UserId} and reminder {ReminderId}", @event.UserId, @event.ReminderId);
        }
    }
}

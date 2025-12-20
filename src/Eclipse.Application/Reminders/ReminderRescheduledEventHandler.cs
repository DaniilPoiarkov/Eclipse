using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

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
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot reschedule reminder {ReminderId} for user {UserId}. {Reason}.", @event.ReminderId, @event.UserId, "User not found");
            return;
        }

        var reminder = user.Reminders.FirstOrDefault(r => r.Id == @event.ReminderId);

        if (reminder is null)
        {
            _logger.LogError("Cannot reschedule reminder {ReminderId} for user {UserId}. {Reason}.", @event.ReminderId, @event.UserId, "Reminder not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var key = $"{nameof(SendReminderJob)}-{@event.UserId}-{@event.ReminderId}";

        var job = JobBuilder.Create<SendReminderJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(
                new SendReminderJobData(
                    reminder.UserId,
                    reminder.Id,
                    reminder.RelatedItemId,
                    user.ChatId,
                    user.Culture,
                    reminder.Text
                ))
            )
            .Build();

        var time = _timeProvider.Now.WithTime(@event.NotifyAt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(key)
            .StartAt(time)
            .Build();

        try
        {
            await scheduler.ScheduleJob(trigger, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reschedule reminder {ReminderId} for user {UserId}.", @event.ReminderId, @event.UserId);
        }
    }
}

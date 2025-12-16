using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class RescheduleRemindersHandler : IEventHandler<UserEnabledDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<RescheduleRemindersHandler> _logger;

    public RescheduleRemindersHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider,
        ILogger<RescheduleRemindersHandler> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(UserEnabledDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("Cannot schedule reminders. User not found.");
            return;
        }

        var remidners = user.Reminders.Select(r => (
            new SendReminderJobData(r.UserId, r.Id, r.RelatedItemId, user.ChatId, user.Culture, r.Text),
            r.NotifyAt)
        );

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var (reminder, notifyAt) in remidners)
        {
            var key = new JobKey($"{nameof(SendReminderJob)}-{user.Id}-{reminder.ReminderId}");

            var job = JobBuilder.Create<SendReminderJob>()
                .WithIdentity(key)
                .UsingJobData("data", JsonConvert.SerializeObject(reminder))
                .Build();

            var time = _timeProvider.Now
                .WithTime(notifyAt);

            var trigger = TriggerBuilder.Create()
                .ForJob(job)
                .StartAt(time)
                .Build();

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
    }
}

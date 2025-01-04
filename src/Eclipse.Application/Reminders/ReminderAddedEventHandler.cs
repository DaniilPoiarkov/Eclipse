using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class ReminderAddedEventHandler : IEventHandler<ReminderAddedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    public ReminderAddedEventHandler(ISchedulerFactory schedulerFactory, ITimeProvider timeProvider)
    {
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(ReminderAddedDomainEvent notification, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var key = new JobKey($"{nameof(SendReminderJob)}-{notification.UserId}-{notification.ReminderId}");

        var job = JobBuilder.Create<SendReminderJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new SendReminderJobData
            {
                ChatId = notification.ChatId,
                Culture = notification.Culture,
                ReminderId = notification.ReminderId,
                Text = notification.Text,
                UserId = notification.UserId,
            }))
            .Build();

        var time = _timeProvider.Now
            .WithTime(notification.NotifyAt);

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

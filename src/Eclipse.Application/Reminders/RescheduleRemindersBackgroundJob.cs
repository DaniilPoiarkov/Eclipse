using Eclipse.Application.Reminders.Sendings;
using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders;

internal sealed class RescheduleRemindersBackgroundJob : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    public RescheduleRemindersBackgroundJob(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var reminders = users.SelectMany(u => u.Reminders.Select(r => (
                new SendReminderJobData(r.UserId, r.Id, r.RelatedItemId, u.ChatId, u.Culture, r.Text),
                r.NotifyAt
            )
        ));

        foreach (var (reminder, notifyAt) in reminders)
        {
            var key = new JobKey($"{nameof(SendReminderJob)}-{reminder.UserId}-{reminder.ReminderId}");

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

using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class RescheduleRemindToFinishTodoItemsJob : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    public RescheduleRemindToFinishTodoItemsJob(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.NotificationsEnabled, cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            var key = new JobKey($"{nameof(RemindToFinishTodoItemsJob)}-{user.Id}");

            var job = JobBuilder.Create<RemindToFinishTodoItemsJob>()
                .WithIdentity(key)
                .UsingJobData("data", JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(user.Id)))
                .Build();

            var time = _timeProvider.Now.WithTime(RemindersConsts.Evening6PM)
                .Add(user.Gmt);

            var trigger = TriggerBuilder.Create()
                .ForJob(job)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInHours(RemindersConsts.OneDayInHours)
                    .RepeatForever()
                )
                .StartAt(time)
                .Build();

            await scheduler.ScheduleJob(job, trigger, cancellationToken);
        }
    }
}

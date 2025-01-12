using Eclipse.Common.Background;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class RescheduleSendGoodMorningJob : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly ISchedulerFactory _schedulerFactory;

    public RescheduleSendGoodMorningJob(IUserRepository userRepository, ITimeProvider timeProvider, ISchedulerFactory schedulerFactory)
    {
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _schedulerFactory = schedulerFactory;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.NotificationsEnabled, cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            var key = JobKey.Create($"{nameof(SendGoodMorningJob)}-{user.Id}");

            var job = JobBuilder.Create<SendGoodMorningJob>()
                .WithIdentity(key)
                .UsingJobData("data", JsonConvert.SerializeObject(new SendGoodMorningJobData(user.Id)))
                .Build();

            var time = _timeProvider.Now.WithTime(RemindersConsts.Morning9AM)
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

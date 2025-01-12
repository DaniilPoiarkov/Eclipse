using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class ScheduleSendGoodMorningJobHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<ScheduleSendGoodMorningJobHandler> _logger;

    public ScheduleSendGoodMorningJobHandler(
        ISchedulerFactory schedulerFactory,
        IUserRepository userRepository,
        ITimeProvider timeProvider,
        ILogger<ScheduleSendGoodMorningJobHandler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(SendGoodMorningJob), @event.UserId, "User not found");
            return;
        }

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

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

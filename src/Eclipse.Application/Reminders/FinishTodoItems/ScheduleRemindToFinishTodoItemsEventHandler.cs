using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class ScheduleRemindToFinishTodoItemsEventHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly ILogger<ScheduleRemindToFinishTodoItemsEventHandler> _logger;

    public ScheduleRemindToFinishTodoItemsEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ITimeProvider timeProvider,
        ILogger<ScheduleRemindToFinishTodoItemsEventHandler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(RemindToFinishTodoItemsJob), @event.UserId, "User not found");
            return;
        }

        var key = JobKey.Create($"{nameof(RemindToFinishTodoItemsJob)}-{@event.UserId}");

        var job = JobBuilder.Create<RemindToFinishTodoItemsJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(@event.UserId)))
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

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

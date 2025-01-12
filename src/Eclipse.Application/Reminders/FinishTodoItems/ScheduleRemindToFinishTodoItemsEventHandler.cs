using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class ScheduleRemindToFinishTodoItemsEventHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<ScheduleRemindToFinishTodoItemsEventHandler> _logger;

    private readonly IJobScheduler<RemindToFinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    public ScheduleRemindToFinishTodoItemsEventHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<ScheduleRemindToFinishTodoItemsEventHandler> logger,
        IJobScheduler<RemindToFinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler)
    {
        _schedulerFactory = schedulerFactory;
        _userRepository = userRepository;
        _logger = logger;
        _jobScheduler = jobScheduler;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(RemindToFinishTodoItemsJob), @event.UserId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler();

        var options = new FinishTodoItemsSchedulerOptions(user.Id, user.Gmt);

        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

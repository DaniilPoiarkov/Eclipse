using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class ScheduleNewUserFinishTodoItemsHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleNewUserFinishTodoItemsHandler> _logger;

    public ScheduleNewUserFinishTodoItemsHandler(IUserRepository userRepository, ISchedulerFactory schedulerFactory, IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler, ILogger<ScheduleNewUserFinishTodoItemsHandler> logger)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
        _logger = logger;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(ScheduleNewUserFinishTodoItemsHandler), @event.UserId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler();

        await _jobScheduler.Schedule(scheduler, new FinishTodoItemsSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

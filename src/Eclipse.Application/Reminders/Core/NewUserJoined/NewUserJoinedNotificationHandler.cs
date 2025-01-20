using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.Core.NewUserJoined;

internal sealed class NewUserJoinedNotificationHandler<TJob, TSchedulerOptions> : IEventHandler<NewUserJoinedDomainEvent>
    where TJob : IJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<NewUserJoinedNotificationHandler<TJob, TSchedulerOptions>> _logger;

    private readonly IJobScheduler<TJob, TSchedulerOptions> _jobScheduler;

    private readonly IOptionsConvertor<User, TSchedulerOptions> _convertor;

    public NewUserJoinedNotificationHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger<NewUserJoinedNotificationHandler<TJob, TSchedulerOptions>> logger,
        IJobScheduler<TJob, TSchedulerOptions> jobScheduler,
        IOptionsConvertor<User, TSchedulerOptions> convertor)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _logger = logger;
        _jobScheduler = jobScheduler;
        _convertor = convertor;
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

        var options = _convertor.Convert(user);

        await _jobScheduler.Schedule(scheduler, options, cancellationToken);
    }
}

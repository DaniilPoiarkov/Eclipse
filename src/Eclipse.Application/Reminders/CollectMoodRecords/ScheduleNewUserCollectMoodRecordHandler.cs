using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.CollectMoodRecords;

internal sealed class ScheduleNewUserCollectMoodRecordHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleNewUserCollectMoodRecordHandler> _logger;

    public ScheduleNewUserCollectMoodRecordHandler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> jobScheduler,
        ILogger<ScheduleNewUserCollectMoodRecordHandler> logger)
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

        await _jobScheduler.Schedule(scheduler, new CollectMoodRecordSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

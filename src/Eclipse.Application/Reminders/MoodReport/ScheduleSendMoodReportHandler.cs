using Eclipse.Common.Events;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.Reminders.MoodReport;

internal sealed class ScheduleSendMoodReportHandler : IEventHandler<NewUserJoinedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IUserRepository _userRepository;

    private readonly IJobScheduler<SendMoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleSendMoodReportHandler> _logger;

    public ScheduleSendMoodReportHandler(
        ISchedulerFactory schedulerFactory,
        IUserRepository userRepository,
        IJobScheduler<SendMoodReportJob, MoodReportSchedulerOptions> jobScheduler,
        ILogger<ScheduleSendMoodReportHandler> logger)
    {
        _schedulerFactory = schedulerFactory;
        _userRepository = userRepository;
        _jobScheduler = jobScheduler;
        _logger = logger;
    }

    public async Task Handle(NewUserJoinedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.FindAsync(@event.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(SendMoodReportJob), @event.UserId, "User not found");
            return;
        }

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        
        await _jobScheduler.Schedule(scheduler, new MoodReportSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

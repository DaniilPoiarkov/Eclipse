using Eclipse.Common.Background;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report;

internal sealed class MoodReportJobRescheduler : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    public MoodReportJobRescheduler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            await _jobScheduler.Schedule(scheduler, new MoodReportSchedulerOptions(user.Id, user.Gmt), cancellationToken);
        }
    }
}

using Eclipse.Common.Background;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Quartz;

namespace Eclipse.Application.Jobs;

internal class JobRescheduler<TJob> : IBackgroundJob
    where TJob : IJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<TJob, SchedulerOptions> _jobScheduler;

    public JobRescheduler(IUserRepository userRepository, ISchedulerFactory schedulerFactory, INotificationScheduler<TJob, SchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetByExpressionAsync(u => u.IsEnabled, cancellationToken);
        var scheduelr = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            await _jobScheduler.Schedule(scheduelr, new SchedulerOptions(user.Id, user.Gmt), cancellationToken);
        }
    }
}

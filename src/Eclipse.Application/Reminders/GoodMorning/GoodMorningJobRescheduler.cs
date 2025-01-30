using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningJobRescheduler : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> _jobScheduler;

    public GoodMorningJobRescheduler(IUserRepository userRepository, ISchedulerFactory schedulerFactory, IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions> jobScheduler)
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
            await _jobScheduler.Schedule(scheduler, new GoodMorningSchedulerOptions(user.Id, user.Gmt), cancellationToken);
        }
    }
}

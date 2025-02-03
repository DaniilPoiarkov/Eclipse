using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Quartz;

namespace Eclipse.Application.Reminders.CollectMoodRecords;

internal sealed class CollectMoodRecordJobRescheduler : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    public CollectMoodRecordJobRescheduler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var scheduelr = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            await _jobScheduler.Schedule(scheduelr, new CollectMoodRecordSchedulerOptions(user.Id, user.Gmt), cancellationToken);
        }
    }
}

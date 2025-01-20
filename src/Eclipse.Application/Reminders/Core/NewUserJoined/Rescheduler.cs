using Eclipse.Common.Background;
using Eclipse.Domain.Users;
using Quartz;

namespace Eclipse.Application.Reminders.Core.NewUserJoined;

internal sealed class Rescheduler<TJob, TSchedulerOptions> : IBackgroundJob
    where TJob : IJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<TJob, TSchedulerOptions> _jobScheduler;

    private readonly IOptionsConvertor<User, TSchedulerOptions> _convertor;

    public Rescheduler(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        IJobScheduler<TJob, TSchedulerOptions> jobScheduler,
        IOptionsConvertor<User, TSchedulerOptions> convertor)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
        _convertor = convertor;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var user in users)
        {
            await _jobScheduler.Schedule(scheduler, _convertor.Convert(user), cancellationToken);
        }
    }
}

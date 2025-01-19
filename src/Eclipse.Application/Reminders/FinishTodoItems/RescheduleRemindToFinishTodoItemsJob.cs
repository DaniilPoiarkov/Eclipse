using Eclipse.Common.Background;
using Eclipse.Domain.Users;

using Quartz;

namespace Eclipse.Application.Reminders.FinishTodoItems;

internal sealed class RescheduleRemindToFinishTodoItemsJob : IBackgroundJob
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<RemindToFinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    public RescheduleRemindToFinishTodoItemsJob(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        IJobScheduler<RemindToFinishTodoItemsJob, FinishTodoItemsSchedulerOptions> jobScheduler)
    {
        _userRepository = userRepository;
        _schedulerFactory = schedulerFactory;
        _jobScheduler = jobScheduler;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        foreach (var options in users.Select(u => new FinishTodoItemsSchedulerOptions(u.Id, u.Gmt)))
        {
            await _jobScheduler.Schedule(scheduler, options, cancellationToken);
        }
    }
}

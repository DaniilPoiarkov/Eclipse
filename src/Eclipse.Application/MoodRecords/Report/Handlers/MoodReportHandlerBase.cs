using Eclipse.Common.Events;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;

using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Handlers;

internal abstract class MoodReportHandlerBase<TEvent> : IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    protected IUserRepository UserRepository { get; }

    protected ISchedulerFactory SchedulerFactory { get; }

    protected ILogger Logger { get; }

    protected INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> JobScheduler { get; }

    protected MoodReportHandlerBase(
        IUserRepository userRepository,
        ISchedulerFactory schedulerFactory,
        ILogger logger,
        INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> jobScheduler)
    {
        UserRepository = userRepository;
        SchedulerFactory = schedulerFactory;
        Logger = logger;
        JobScheduler = jobScheduler;
    }

    public abstract Task Handle(TEvent @event, CancellationToken cancellationToken = default);

    protected async Task Handle(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await UserRepository.FindAsync(userId, cancellationToken);

        if (user is null)
        {
            Logger.LogError("Cannot scheduler {Job} job for user {UserId}. Reason: {Reason}", nameof(ScheduleNewUserMoodReportHandler), userId, "User not found");
            return;
        }

        var scheduler = await SchedulerFactory.GetScheduler();

        await JobScheduler.Schedule(scheduler, new MoodReportSchedulerOptions(user.Id, user.Gmt), cancellationToken);
    }
}

using Quartz;

namespace Eclipse.Common.Notifications;

public interface INotificationScheduler<TJob, TOptions>
    where TJob : IJob
{
    Task Schedule(IScheduler scheduler, TOptions options, CancellationToken cancellationToken = default);

    Task Unschedule(IScheduler scheduler, TOptions options, CancellationToken cancellationToken = default);
}

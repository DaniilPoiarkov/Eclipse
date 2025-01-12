using Quartz;

namespace Eclipse.Application.Reminders;

internal interface IJobScheduler<TJob, TOptions>
    where TJob : IJob
{
    Task Schedule(IScheduler scheduler, TOptions options, CancellationToken cancellationToken = default);
}

using Quartz;

namespace Eclipse.Application.Reminders.Core;

internal interface IJobScheduler<TJob, TOptions>
    where TJob : IJob
{
    Task Schedule(IScheduler scheduler, TOptions options, CancellationToken cancellationToken = default);

    Task Unschedule(IScheduler scheduler, TOptions options, CancellationToken cancellationToken = default);
}

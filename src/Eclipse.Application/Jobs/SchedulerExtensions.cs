using Quartz;

namespace Eclipse.Application.Jobs;

internal static class SchedulerExtensions
{
    internal static async Task<IJobDetail> GetJob<TJob>(this IScheduler scheduler, CancellationToken cancellationToken = default)
        where TJob : IJob, IJobWithKey
    {
        var job = await scheduler.GetJobDetail(TJob.Key, cancellationToken);

        job ??= JobBuilder.Create<TJob>()
            .WithIdentity(TJob.Key)
            .Build();
        
        return job;
    }
}

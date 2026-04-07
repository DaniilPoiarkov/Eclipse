using Quartz;

namespace Eclipse.Application.Jobs;

internal static class SchedulerExtensions
{
    internal static async Task<IJobDetail> GetOrAddDurableJob<TJob>(this IScheduler scheduler, CancellationToken cancellationToken = default)
        where TJob : IJob, IJobWithKey
    {
        var job = await scheduler.GetJobDetail(TJob.Key, cancellationToken);

        if (job is null)
        {
            job ??= JobBuilder.Create<TJob>()
                .WithIdentity(TJob.Key)
                .StoreDurably()
                .Build();

            await scheduler.AddJob(job, replace: true, cancellationToken);
        }

        return job;
    }
}

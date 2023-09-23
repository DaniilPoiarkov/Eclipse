using Eclipse.Infrastructure.Quartz;

using Quartz;

namespace Eclipse.Infrastructure.Internals.Quartz;

internal class EclipseScheduler : IEclipseScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    public EclipseScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task ScheduleJob(IJobConfiguration configuration)
    {
        var job = configuration.BuildJob();
        var trigger = configuration.BuildTrigger();

        var scheduler = await _schedulerFactory.GetScheduler();

        await scheduler.ScheduleJob(job, trigger);
    }

    public async Task DeleteJob(JobKey key)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        await scheduler.DeleteJob(key);
    }
}

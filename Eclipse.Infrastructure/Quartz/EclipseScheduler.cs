using Quartz;

namespace Eclipse.Infrastructure.Quartz;

internal class EclipseScheduler : IEclipseScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    private bool IsInitialized = false;

    private IScheduler? _scheduler;

    public EclipseScheduler(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task ScheduleJob(IJobConfiguration configuration)
    {
        await EnsureInitialized();

        var job = configuration.BuildJob();
        var trigger = configuration.BuildTrigger();

        await _scheduler!.ScheduleJob(job, trigger);
    }

    public async Task DeleteJob(JobKey key)
    {
        await EnsureInitialized();

        await _scheduler!.DeleteJob(key);
    }

    private async Task EnsureInitialized()
    {
        if (IsInitialized)
        {
            return;
        }

        await Initialize();
    }

    public async Task Initialize()
    {
        _scheduler = await _schedulerFactory.GetScheduler();

        if (!_scheduler.IsStarted)
        {
            await _scheduler.Start();
        }

        IsInitialized = true;
    }
}

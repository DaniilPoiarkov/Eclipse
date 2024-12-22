using Eclipse.Common.Background;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Infrastructure.Background;

internal sealed class BackgroundJobManager : IBackgroundJobManager
{
    private readonly ISchedulerFactory _schedulerFactory;

    public BackgroundJobManager(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task EnqueueAsync<TBackgroundJob, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TBackgroundJob : IBackgroundJob<TArgs>
    {
        var job = JobBuilder.Create<OneOffJobProcessor<TBackgroundJob, TArgs>>()
            .UsingJobData("args", JsonConvert.SerializeObject(args))
            .Build();

        await ScheduleJob(job, cancellationToken);
    }

    public async Task EnqueueAsync<TBackgroundJob>(CancellationToken cancellationToken = default)
        where TBackgroundJob : IBackgroundJob
    {
        var job = JobBuilder.Create<OneOffJobProcessor<TBackgroundJob>>()
            .Build();

        await ScheduleJob(job, cancellationToken);
    }

    private async Task ScheduleJob(IJobDetail job, CancellationToken cancellationToken)
    {
        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartNow()
            .Build();

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

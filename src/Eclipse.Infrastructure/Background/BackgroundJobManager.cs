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

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .StartNow()
            .Build();

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}

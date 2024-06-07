using Eclipse.Common.Background;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Infrastructure.Background;

internal sealed class OneOffJobProcessor<TBackgroundJob, TArgs> : IJob
    where TBackgroundJob : IBackgroundJob<TArgs>
{
    private static readonly int _maxRefireCount = 10;

    private readonly TBackgroundJob _job;

    public OneOffJobProcessor(TBackgroundJob job)
    {
        _job = job;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > _maxRefireCount)
        {
            return;
        }

        var data = context.MergedJobDataMap.GetString("args");

        var args = JsonConvert.DeserializeObject<TArgs>(data!)!;

        try
        {
            await _job.ExecureAsync(args, context.CancellationToken);

            await context.Scheduler.UnscheduleJob(context.Trigger.Key);
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(
                msg: $"Failed to process job {typeof(TBackgroundJob).Name}.",
                refireImmediately: true,
                cause: ex
            );
        }
    }
}

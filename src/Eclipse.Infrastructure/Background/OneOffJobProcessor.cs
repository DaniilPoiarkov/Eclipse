using Eclipse.Common.Background;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Infrastructure.Background;

internal sealed class OneOffJobProcessor<TBackgroundJob, TArgs> : IJob
    where TBackgroundJob : IBackgroundJob<TArgs>
{
    private static readonly int _maxRefireCount = 10;

    private readonly TBackgroundJob _job;

    private readonly ILogger<OneOffJobProcessor<TBackgroundJob, TArgs>> _logger;

    public OneOffJobProcessor(TBackgroundJob job, ILogger<OneOffJobProcessor<TBackgroundJob, TArgs>> logger)
    {
        _job = job;
        _logger = logger;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (context.RefireCount > _maxRefireCount)
        {
            _logger.LogError("Exceeded refire count for job {job}.", typeof(TBackgroundJob).Name);
            return;
        }

        var data = context.MergedJobDataMap.GetString("args");

        var args = JsonConvert.DeserializeObject<TArgs>(data!)!;

        try
        {
            await _job.ExecuteAsync(args, cancellationToken);

            await context.Scheduler.UnscheduleJob(context.Trigger.Key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to process {Job} job. Error: {Error}", typeof(TBackgroundJob).Name, ex);

            throw new JobExecutionException($"Failed to process job {typeof(TBackgroundJob).Name}.", ex)
            {
                RefireImmediately = true
            };
        }
    }
}

internal sealed class OneOffJobProcessor<TBackgroundJob> : IJob
    where TBackgroundJob : IBackgroundJob
{
    private static readonly int _maxRefireCount = 10;

    private readonly TBackgroundJob _job;

    private readonly ILogger<OneOffJobProcessor<TBackgroundJob>> _logger;

    public OneOffJobProcessor(TBackgroundJob job, ILogger<OneOffJobProcessor<TBackgroundJob>> logger)
    {
        _job = job;
        _logger = logger;
    }

    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        if (context.RefireCount > _maxRefireCount)
        {
            _logger.LogError("Exceeded refire count for job {job}.", typeof(TBackgroundJob).Name);
            return;
        }

        try
        {
            await _job.Execute(cancellationToken);

            await context.Scheduler.UnscheduleJob(context.Trigger.Key, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to process {Job} job. Error: {Error}", typeof(TBackgroundJob).Name, ex);

            throw new JobExecutionException($"Failed to process job {typeof(TBackgroundJob).Name}.", ex)
            {
                RefireImmediately = true
            };
        }
    }
}

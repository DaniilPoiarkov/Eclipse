﻿using Eclipse.Common.Background;

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

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > _maxRefireCount)
        {
            _logger.LogError("Exceded refire count for job {job}.", typeof(TBackgroundJob).Name);
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
                refireImmediately: false,
                cause: ex
            );
        }
    }
}

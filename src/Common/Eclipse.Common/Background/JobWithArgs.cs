using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Common.Background;

public abstract class JobWithArgs<TArgs> : IJob
{
    protected readonly ILogger Logger;

    protected JobWithArgs(ILogger logger)
    {
        Logger = logger;
    }

    public virtual async Task Execute(IJobExecutionContext context)
    {
        var data = context.MergedJobDataMap.GetString("data");

        if (data.IsNullOrEmpty())
        {
            Logger.LogError("Cannot deserialize event with data {Data}", "{null}");
            return;
        }

        var args = JsonConvert.DeserializeObject<TArgs>(data);

        if (args is null)
        {
            Logger.LogError("Cannot deserialize event with data {Data}", data);
            return;
        }

        await Execute(args, context.CancellationToken);
    }

    protected abstract Task Execute(TArgs args, CancellationToken cancellationToken);
}

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed class CollectMoodRecordJob : IJob
{
    private readonly IMoodRecordCollector _collector;

    private readonly ILogger<CollectMoodRecordJob> _logger;

    public CollectMoodRecordJob(IMoodRecordCollector collector, ILogger<CollectMoodRecordJob> logger)
    {
        _collector = collector;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var data = context.MergedJobDataMap.GetString("data");

        if (data.IsNullOrEmpty())
        {
            _logger.LogError("Cannot deserialize event with data {Data}", "{null}");
            return;
        }

        var args = JsonConvert.DeserializeObject<CollectMoodRecordJobData>(data);

        if (args is null)
        {
            _logger.LogError("Cannot deserialize event with data {Data}", data);
            return;
        }

        await _collector.CollectAsync(args.UserId, context.CancellationToken);
    }
}

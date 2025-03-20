using Eclipse.Common.Background;

using Microsoft.Extensions.Logging;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed class CollectMoodRecordJob : JobWithArgs<CollectMoodRecordJobData>
{
    private readonly IMoodRecordCollector _collector;

    public CollectMoodRecordJob(IMoodRecordCollector collector, ILogger<CollectMoodRecordJob> logger) : base(logger)
    {
        _collector = collector;
    }

    protected override Task Execute(CollectMoodRecordJobData args, CancellationToken cancellationToken)
    {
        return _collector.CollectAsync(args.UserId, cancellationToken);
    }
}

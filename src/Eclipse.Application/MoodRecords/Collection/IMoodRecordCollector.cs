namespace Eclipse.Application.MoodRecords.Collection;

public interface IMoodRecordCollector
{
    Task CollectAsync(Guid userId, CancellationToken cancellationToken = default);
}

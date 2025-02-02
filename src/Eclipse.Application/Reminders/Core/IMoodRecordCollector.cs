namespace Eclipse.Application.Reminders.Core;

public interface IMoodRecordCollector
{
    Task CollectAsync(Guid userId, CancellationToken cancellationToken = default);
}

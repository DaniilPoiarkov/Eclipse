namespace Eclipse.Application.Contracts.OutboxMessages;

[Serializable]
public sealed class ProcessOutboxMessagesResult
{
    public int TotalCount { get; }

    public int ProcessedCount { get; }

    public int FailedCount { get; }

    public IReadOnlyCollection<string> OcurredErrors { get; }

    public ProcessOutboxMessagesResult(int totalCount, int processedCount, int failedCount, IEnumerable<string> occuredErrors)
    {
        TotalCount = totalCount;
        ProcessedCount = processedCount;
        FailedCount = failedCount;
        OcurredErrors = occuredErrors.ToArray();
    }

    private static readonly ProcessOutboxMessagesResult _empty = new(0, 0, 0, []);
    public static ProcessOutboxMessagesResult Empty => _empty;
}

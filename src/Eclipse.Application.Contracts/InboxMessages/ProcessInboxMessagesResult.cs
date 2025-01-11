namespace Eclipse.Application.Contracts.InboxMessages;

public sealed class ProcessInboxMessagesResult
{
    public int TotalCount { get; init; }
    public int ProcessCount { get; init; }
    public int FailedCount { get; init; }

    public IEnumerable<string> Errors { get; init; }

    public ProcessInboxMessagesResult(int totalCount, int processCount, int failedCount, IEnumerable<string> errors)
    {
        TotalCount = totalCount;
        ProcessCount = processCount;
        FailedCount = failedCount;
        Errors = errors;
    }

    public static readonly ProcessInboxMessagesResult Empty = new(0, 0, 0, []);
}

namespace Eclipse.Application.Contracts.InboxMessages;

public sealed class OutboxToInboxConversionResult
{
    public int TotalCount { get; init; }
    public int ConvertedCount { get; init; }
    public int FailedCount { get; init; }

    public IEnumerable<string> Errors { get; init; }

    public OutboxToInboxConversionResult(int totalCount, int convertedCount, int failedCount, IEnumerable<string> errors)
    {
        TotalCount = totalCount;
        ConvertedCount = convertedCount;
        FailedCount = failedCount;
        Errors = errors;
    }

    public static readonly OutboxToInboxConversionResult Empty = new(0, 0, 0, []);
}

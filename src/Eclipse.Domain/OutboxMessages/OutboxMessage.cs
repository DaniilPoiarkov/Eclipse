using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.OutboxMessages;

public sealed class OutboxMessage : Entity
{
    public string Type { get; private set; }

    public string JsonContent { get; private set; }

    public string? Error { get; private set; }

    public DateTime OccuredAt { get; init; }

    public DateTime? ProcessedAt { get; private set; }

    public OutboxMessage(Guid id, string type, string jsonContent, DateTime occuredAt) : base(id)
    {
        Type = type;
        JsonContent = jsonContent;
        OccuredAt = occuredAt;
    }

    public void SetError(string error, DateTime processedAt)
    {
        Error = error;
        ProcessedAt = processedAt;
    }

    public void SetProcessed(DateTime processedAt)
    {
        Error = null;
        ProcessedAt = processedAt;
    }
}

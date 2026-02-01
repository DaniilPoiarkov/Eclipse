using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.InboxMessages;

namespace Eclipse.Domain.InboxMessages;

public sealed class InboxMessage : Entity
{
    public Guid OutboxMessageId { get; init; }

    public string HandlerName { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;

    public string? Error { get; private set; }

    public InboxMessageStatus Status { get; private set; }

    public DateTime OccuredAt { get; init; }
    public DateTime? ProcessedAt { get; private set; }

    private InboxMessage(Guid id, Guid outboxMessageId, string handlerName, string payload, string type, InboxMessageStatus status, DateTime occuredAt) : base(id)
    {
        OutboxMessageId = outboxMessageId;
        HandlerName = handlerName;
        Payload = payload;
        Type = type;
        Status = status;
        OccuredAt = occuredAt;
    }

    private InboxMessage() { }

    public static InboxMessage Create(Guid id, Guid outboxMessageId, string handlerName, string payload, string type, DateTime occuredAt)
    {
        return new InboxMessage(id, outboxMessageId, handlerName, payload, type, InboxMessageStatus.Pending, occuredAt);
    }

    public void SetProcessed(DateTime processedAt)
    {
        ProcessedAt = processedAt;
        Error = null;
        Status = InboxMessageStatus.Processed;
    }

    public void SetError(string error, DateTime processedAt)
    {
        Error = error;
        ProcessedAt = processedAt;
        Status = InboxMessageStatus.Failed;
    }

    public void SetInProcess()
    {
        Status = InboxMessageStatus.InProcess;
    }

    public void Reset()
    {
        if (Status != InboxMessageStatus.Failed)
        {
            throw new InvalidOperationException("Cannot reset a message that is not in Failed status.");
        }

        Status = InboxMessageStatus.Pending;
    }
}

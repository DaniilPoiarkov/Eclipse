using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.PromotionLogs;

public sealed class PromotionLog : Entity
{
    public Guid PromotionId { get; init; }

    public Guid ReceiverId { get; init; }

    public string Message { get; init; }

    public bool ReceivedSuccessfully { get; init; }

    public PromotionLog(Guid id, Guid promotionId, Guid receiverId, string message, bool receivedSuccessfully) : base(id)
    {
        PromotionId = promotionId;
        ReceiverId = receiverId;
        Message = message;
        ReceivedSuccessfully = receivedSuccessfully;
    }
}

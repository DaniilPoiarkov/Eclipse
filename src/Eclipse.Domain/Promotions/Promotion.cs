using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Promotions;

namespace Eclipse.Domain.Promotions;

public sealed class Promotion : AggregateRoot, IHasCreatedAt
{
    public long FromChatId { get; init; }

    public int MessageId { get; private set; }

    public string? InlineButtonText { get; private set; }

    public PromotionStatus Status { get; private set; }

    public DateTime CreatedAt { get; init; }

    private Promotion(Guid id, long fromChatId, int messageId, string? inlineButtonText, PromotionStatus status, DateTime createdAt) : base(id)
    {
        FromChatId = fromChatId;
        MessageId = messageId;
        InlineButtonText = inlineButtonText;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Promotion Create(long fromChatId, int messageId, string? inlineButtonText, DateTime createdAt)
    {
        return new Promotion(Guid.CreateVersion7(), fromChatId, messageId, inlineButtonText, PromotionStatus.Pending, createdAt);
    }

    public void SetInlineButton(string inlineButtonText)
    {
        if (inlineButtonText.IsNullOrEmpty())
        {
            throw new ArgumentException("Inline button text is required.", nameof(inlineButtonText));
        }

        InlineButtonText = inlineButtonText;
    }

    public void RemoveInlineButton()
    {
        InlineButtonText = null;
    }

    public void Publish()
    {
        Status = PromotionStatus.Published;
        AddEvent(new PromotionPublishedDomainEvent(Id));
    }
}

using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Promotions;

namespace Eclipse.Domain.Promotions;

public sealed class Promotion : AggregateRoot, IHasCreatedAt
{
    public long FromChatId { get; init; }

    public int MessageId { get; private set; }

    public string InlineButtonText { get; private set; }

    public int TimesPublished { get; private set; }

    public PromotionStatus Status { get; private set; }

    public DateTime CreatedAt { get; init; }

    private Promotion(Guid id, long fromChatId, int messageId, string inlineButtonText, int timesPublished, PromotionStatus status, DateTime createdAt) : base(id)
    {
        FromChatId = fromChatId;
        MessageId = messageId;
        InlineButtonText = inlineButtonText;
        TimesPublished = timesPublished;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Promotion Create(long fromChatId, int messageId, string inlineButtonText, DateTime createdAt)
    {
        return new Promotion(Guid.CreateVersion7(), fromChatId, messageId, inlineButtonText, 0, PromotionStatus.Pending, createdAt);
    }

    public void SetInlineButton(string inlineButtonText)
    {
        if (inlineButtonText.IsNullOrEmpty())
        {
            throw new ArgumentException("Inline button text is required.", nameof(inlineButtonText));
        }

        InlineButtonText = inlineButtonText;
    }

    public void RequestPublishing()
    {
        if (!CanBeTransitioned(PromotionStatus.PublishingRequested))
        {
            throw new InvalidOperationException("Cannot request publishing when promotion is currently in publishing.");
        }

        Status = PromotionStatus.PublishingRequested;
        AddEvent(new PromotionPublishingRequestedDomainEvent(Id));
    }

    public void StartPublishing()
    {
        if (!CanBeTransitioned(PromotionStatus.InPublishing))
        {
            throw new InvalidOperationException("Cannot start publishing when promotion publishing is not in requested state.");
        }

        Status = PromotionStatus.InPublishing;
    }

    public void FinishPublishing()
    {
        if (!CanBeTransitioned(PromotionStatus.Published))
        {
            throw new InvalidOperationException("Cannot finish publishing as promotion is not in process of publishing state.");
        }

        TimesPublished++;
        Status = PromotionStatus.Published;
    }

    public bool CanBeTransitioned(PromotionStatus status)
    {
        return status switch
        {
            PromotionStatus.Pending => false,
            PromotionStatus.PublishingRequested => Status is not PromotionStatus.InPublishing,
            PromotionStatus.InPublishing => Status is PromotionStatus.PublishingRequested,
            PromotionStatus.Published => Status is PromotionStatus.InPublishing,
            _ => false
        };
    }
}

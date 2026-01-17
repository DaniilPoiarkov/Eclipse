using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Promotions;

namespace Eclipse.Domain.Promotions;

public sealed class Promotion : AggregateRoot, IHasCreatedAt
{
    public string Title { get; private set; }

    public long FromChatId { get; init; }

    public int MessageId { get; private set; }

    public string InlineButtonText { get; init; }
    public string InlineButtonLink { get; init; }

    public int TimesPublished { get; private set; }

    public PromotionStatus Status { get; private set; }

    public DateTime CreatedAt { get; init; }

    private Promotion(Guid id, string title, long fromChatId, int messageId, string inlineButtonText, string inlineButtonLink, int timesPublished, PromotionStatus status, DateTime createdAt) : base(id)
    {
        Title = title;
        FromChatId = fromChatId;
        MessageId = messageId;
        InlineButtonText = inlineButtonText;
        InlineButtonLink = inlineButtonLink;
        TimesPublished = timesPublished;
        Status = status;
        CreatedAt = createdAt;
    }

    public static Promotion Create(string title, long fromChatId, int messageId, string inlineButtonText, string inlineButtonLink, DateTime createdAt)
    {
        return new Promotion(Guid.CreateVersion7(), title, fromChatId, messageId, inlineButtonText, inlineButtonLink, 0, PromotionStatus.Pending, createdAt);
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

    public void SetTitle(string title)
    {
        if (title.IsNullOrEmpty())
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Title = title;
    }
}

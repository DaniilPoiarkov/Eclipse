using Eclipse.Domain.Shared.Promotions;

namespace Eclipse.Domain.Promotions;

public static class PromotionExtensions
{
    extension(Promotion promotion)
    {
        public bool HasInlineButton => !promotion.InlineButtonText.IsNullOrEmpty();

        public bool CanStartPublishing => promotion.CanBeTransitioned(PromotionStatus.InPublishing);

        public bool CanRequestPublishing => promotion.CanBeTransitioned(PromotionStatus.PublishingRequested);
    }
}

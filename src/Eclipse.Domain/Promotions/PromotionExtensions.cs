namespace Eclipse.Domain.Promotions;

public static class PromotionExtensions
{
    extension(Promotion promotion)
    {
        public bool HasInlineButton => !promotion.InlineButtonText.IsNullOrEmpty();
    }
}

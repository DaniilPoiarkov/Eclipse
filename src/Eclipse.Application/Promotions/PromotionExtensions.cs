using Eclipse.Application.Contracts.Promotions;
using Eclipse.Domain.Promotions;

namespace Eclipse.Application.Promotions;

internal static class PromotionExtensions
{
    public static PromotionDto ToDto(this Promotion promotion)
    {
        return new PromotionDto(
            promotion.Id,
            promotion.Title,
            promotion.FromChatId,
            promotion.MessageId,
            promotion.InlineButtonText,
            promotion.TimesPublished,
            promotion.Status,
            promotion.CreatedAt
        );
    }
}

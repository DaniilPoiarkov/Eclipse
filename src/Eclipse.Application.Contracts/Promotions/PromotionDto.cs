using Eclipse.Domain.Shared.Promotions;

namespace Eclipse.Application.Contracts.Promotions;

public sealed record PromotionDto(
    Guid Id,
    long FromChatId,
    int MessageId,
    string? InlineButtonText,
    int TimesPublished,
    PromotionStatus Status,
    DateTime CreatedAt
);

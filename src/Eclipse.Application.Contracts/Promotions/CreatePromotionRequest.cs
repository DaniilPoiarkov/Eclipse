namespace Eclipse.Application.Contracts.Promotions;

public sealed record CreatePromotionRequest(
    long FromChatId,
    int MessageId,
    string InlineButtonText
);

namespace Eclipse.Application.Contracts.Promotions;

public sealed record CreatePromotionRequest(
    string Title,
    long FromChatId,
    int MessageId,
    string InlineButtonText
);

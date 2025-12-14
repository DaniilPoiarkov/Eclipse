namespace Eclipse.Application.Contracts.Promotions;

public sealed record SendPromotionRequest(
    long FromChatId,
    int MessageId
);

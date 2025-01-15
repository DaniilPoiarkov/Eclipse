namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

internal sealed class SendPromotionBackgroundJobArgs
{
    public int MessageId { get; init; }

    public long FromChatId { get; init; }
}

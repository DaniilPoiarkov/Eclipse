namespace Eclipse.Application.Reminders;

internal sealed class SendReminderJobData
{
    public Guid UserId { get; init; }
    public Guid ReminderId { get; init; }
    public Guid? RelatedItemId { get; init; }

    public long ChatId { get; init; }

    public required string Culture { get; init; }
    public required string Text { get; init; }
}

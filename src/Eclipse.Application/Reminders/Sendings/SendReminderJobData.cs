namespace Eclipse.Application.Reminders.Sendings;

internal sealed record SendReminderJobData(
    Guid UserId,
    Guid ReminderId,
    Guid? RelatedItemId,
    long ChatId,
    string Culture,
    string Text
);

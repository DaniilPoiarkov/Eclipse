namespace Eclipse.Application.Reminders.Sendings;

public sealed record ReminderArguments(
    Guid ReminderId,
    Guid UserId,
    Guid? RelatedItemId,
    string Text,
    string Culture,
    long ChatId
);

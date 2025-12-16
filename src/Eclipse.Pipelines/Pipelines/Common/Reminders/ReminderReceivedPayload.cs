namespace Eclipse.Pipelines.Pipelines.Common.Reminders;

internal sealed record ReminderReceivedPayload(
    Guid ReminderId,
    Guid UserId,
    Guid TodoItemId,
    string Text
);

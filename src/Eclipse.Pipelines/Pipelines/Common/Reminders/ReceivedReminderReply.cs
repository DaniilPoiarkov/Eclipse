namespace Eclipse.Pipelines.Pipelines.Common.Reminders;

internal sealed record ReceivedReminderReply(
    Guid ReminderId,
    Guid TodoItemId,
    Guid UserId,
    ReminderReceivedReplyAction Action
);

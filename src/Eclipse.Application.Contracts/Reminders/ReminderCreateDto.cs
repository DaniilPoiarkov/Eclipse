namespace Eclipse.Application.Contracts.Reminders;

public record ReminderCreateDto(
    Guid? RelatedItemId,
    string Text,
    TimeOnly NotifyAt
);

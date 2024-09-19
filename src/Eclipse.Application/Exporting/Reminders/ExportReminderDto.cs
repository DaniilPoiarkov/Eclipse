namespace Eclipse.Application.Exporting.Reminders;

internal record ExportReminderDto(Guid Id, Guid UserId, string Text, TimeOnly NotifyAt);

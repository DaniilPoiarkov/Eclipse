namespace Eclipse.Application.Exporting.Models;

internal record ExportReminderDto(Guid Id, Guid UserId, string Text, TimeOnly NotifyAt);

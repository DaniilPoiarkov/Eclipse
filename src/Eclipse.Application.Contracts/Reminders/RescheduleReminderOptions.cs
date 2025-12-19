namespace Eclipse.Application.Contracts.Reminders;

public sealed record RescheduleReminderOptions(bool ReminderReceived, DateTime NotifyAt);

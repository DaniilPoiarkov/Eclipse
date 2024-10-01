namespace Eclipse.Application.Contracts.Reminders;

public class ReminderCreateDto
{
    public string Text { get; set; } = string.Empty;

    public TimeOnly NotifyAt { get; set; }
}

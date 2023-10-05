namespace Eclipse.Application.Contracts.Reminders;

public class ReminderCreateDto
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public TimeOnly NotifyAt { get; set; }
}

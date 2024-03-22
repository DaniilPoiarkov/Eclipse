using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.Reminders;

public class ReminderDto : EntityDto
{
    public Guid UserId { get; set; }

    public string Text { get; set; } = string.Empty;

    public TimeOnly NotifyAt { get; set; }
}

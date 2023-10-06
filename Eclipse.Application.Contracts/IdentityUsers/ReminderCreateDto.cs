namespace Eclipse.Application.Contracts.IdentityUsers;

public class ReminderCreateDto
{
    public string Text { get; set; } = string.Empty;

    public TimeOnly NotifyAt { get; set; }
}

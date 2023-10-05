using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Contracts.Reminders;

public class ReminderWithUserDto
{
    public string Text { get; set; } = string.Empty;

    public TimeOnly NotifyAt { get; set; }

    public required IdentityUserDto User { get; set; }
}

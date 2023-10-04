using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Contracts.Notifications;

public class NotificationWithUserDto
{
    public string Name { get; set; } = string.Empty;

    public DateTime NotifyAt { get; set; }

    public required IdentityUserDto User { get; set; }
}

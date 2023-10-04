using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.Notifications;

public class NotificationDto : EntityDto
{
    public Guid UserId { get; set; }

    public long UserChatId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime NotifyAt { get; set; }
}

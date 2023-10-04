namespace Eclipse.Application.Contracts.Notifications;

public class NotificationCreateDto
{
    public Guid UserId { get; set; }

    public long UserChatId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime NotifyAt { get; set; }
}

using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.Notifications;

public class Notification : Entity
{
    public Guid UserId { get; set; }

    public long UserChatId { get; set; }

    public string Name { get; set; }

    public DateTime NotifyAt { get; set; }

    [JsonConstructor]
    internal Notification(Guid id, Guid userId, long userChatId, string name, DateTime notifyAt) : base(id)
    {
        UserId = userId;
        UserChatId = userChatId;
        Name = name;
        NotifyAt = notifyAt;
    }
}

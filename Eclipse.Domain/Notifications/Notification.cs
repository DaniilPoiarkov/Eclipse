using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.Notifications;

public class Notification : Entity
{
    public Guid UserId { get; private set; }

    public long UserChatId { get; private set; }

    public string Name { get; private set; }

    public DateTime NotifyAt { get; private set; }

    [JsonConstructor]
    internal Notification(Guid id, Guid userId, long userChatId, string name, DateTime notifyAt) : base(id)
    {
        UserId = userId;
        UserChatId = userChatId;
        Name = name;
        NotifyAt = notifyAt;
    }
}

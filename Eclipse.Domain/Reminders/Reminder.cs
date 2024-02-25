using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.Reminders;

public sealed class Reminder : Entity
{
    public Guid UserId { get; private set; }

    public string Text { get; private set; }

    public TimeOnly NotifyAt { get; private set; }

    [JsonConstructor]
    internal Reminder(Guid id, Guid userId, string text, TimeOnly notifyAt) : base(id)
    {
        UserId = userId;
        Text = text;
        NotifyAt = notifyAt;
    }
}

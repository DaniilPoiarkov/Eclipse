﻿using Eclipse.Domain.Shared.Entities;

using Newtonsoft.Json;

namespace Eclipse.Domain.Reminders;

public class Reminder : Entity
{
    public Guid UserId { get; private set; }

    public string Text { get; private set; }

    public TimeOnly NotifyAt { get; private set; }

    [JsonConstructor]
    public Reminder(Guid id, Guid userId, string text, TimeOnly notifyAt) : base(id)
    {
        UserId = userId;
        Text = text;
        NotifyAt = notifyAt;
    }
}

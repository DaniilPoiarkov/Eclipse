﻿using Eclipse.Core.Models;

namespace Eclipse.Core.Core;

/// <summary>
/// Contains information scoped to current request
/// </summary>
public sealed class MessageContext
{
    public string Value { get; }

    public long ChatId { get; }

    public TelegramUser User { get; }

    internal IServiceProvider Services { get; set; } = null!;

    public MessageContext(long chatId, string value, TelegramUser user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ChatId = chatId;
        Value = value;
        User = user;
    }
}

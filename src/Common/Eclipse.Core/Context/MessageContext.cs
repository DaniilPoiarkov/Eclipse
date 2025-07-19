namespace Eclipse.Core.Context;

/// <summary>
/// Contains information scoped to current request
/// </summary>
public sealed class MessageContext
{
    public string Value { get; }

    public long ChatId { get; }

    public TelegramUser User { get; }

    internal IServiceProvider Services { get; }

    public MessageContext(long chatId, string value, TelegramUser user, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ChatId = chatId;
        Value = value;
        User = user;
        Services = serviceProvider;
    }
}

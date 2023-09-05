using Eclipse.Core.Models;

namespace Eclipse.Core.Core;

public class MessageContext
{
    public string Value { get; }
    public long ChatId { get; }

    public TelegramUser User { get; }

    public MessageContext(long chatId, string value, TelegramUser user)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));
        ChatId = chatId;
        Value = value;
        User = user;
    }
}

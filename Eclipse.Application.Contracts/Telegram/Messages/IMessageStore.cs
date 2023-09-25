using Telegram.Bot.Types;

namespace Eclipse.Application.Contracts.Telegram.Messages;

public interface IMessageStore
{
    void Set(MessageKey key, Message message);

    Message? GetMessage(MessageKey key);
}

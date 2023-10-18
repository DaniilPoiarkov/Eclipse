using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Messages;

public interface IMessageStore
{
    void Set(MessageKey key, Message message);

    Message? GetMessage(MessageKey key);
}

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Messages;

public interface IMessageStore : IStore<Message, MessageKey>
{
    
}

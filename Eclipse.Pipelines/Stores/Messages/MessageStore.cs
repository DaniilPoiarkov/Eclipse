using Eclipse.Infrastructure.Cache;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Messages;

internal class MessageStore : StoreBase<Message, MessageKey>, IMessageStore
{
    public MessageStore(ICacheService cacheService) : base(cacheService)
    {
    }
}

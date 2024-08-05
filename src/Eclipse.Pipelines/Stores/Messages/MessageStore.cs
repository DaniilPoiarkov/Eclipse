using Eclipse.Common.Caching;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Messages;

internal sealed class MessageStore : StoreBase<Message, MessageKey>, IMessageStore
{
    public MessageStore(ICacheService cacheService) : base(cacheService)
    {
    }
}

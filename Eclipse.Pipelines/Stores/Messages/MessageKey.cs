using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Stores.Messages;

public class MessageKey : StoreKey
{
    public MessageKey(long chatId) : base(chatId)
    {
    }

    public override CacheKey ToCacheKey()
    {
        return new CacheKey($"Message-{ChatId}");
    }
}

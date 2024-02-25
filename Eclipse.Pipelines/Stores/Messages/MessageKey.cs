using Eclipse.Common.Cache;

namespace Eclipse.Pipelines.Stores.Messages;

public sealed class MessageKey : StoreKey
{
    public MessageKey(long chatId) : base(chatId)
    {
    }

    public override CacheKey ToCacheKey()
    {
        return new CacheKey($"Message-{ChatId}");
    }
}

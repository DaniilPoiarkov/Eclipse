﻿using Eclipse.Common.Caching;

namespace Eclipse.Pipelines.Stores;

public abstract class StoreKey
{
    public long ChatId { get; }

    public StoreKey(long chatId)
    {
        ChatId = chatId;
    }

    public abstract CacheKey ToCacheKey();
}

﻿namespace Eclipse.Infrastructure.Cache;

public class CacheKey
{
    public string Key { get; }

    public CacheKey(string key)
    {
        Key = key;
    }
}

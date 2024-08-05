﻿using Eclipse.Common.Caching;

using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json;

namespace Eclipse.Infrastructure.Cache;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAndDeleteAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<T>(key, cancellationToken);

        await DeleteAsync(key, cancellationToken);

        return value;
    }

    public async Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetStringAsync(key.Key, cancellationToken);

        if (json.IsNullOrEmpty())
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        });
    }

    public Task SetAsync<T>(CacheKey key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        return _cache.SetStringAsync(key.Key, json, options, cancellationToken);
    }

    public Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key.Key, cancellationToken);
    }
}

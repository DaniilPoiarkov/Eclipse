using Eclipse.Common.Caching;

using Microsoft.Extensions.Caching.Hybrid;

namespace Eclipse.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly HybridCache _cache;

    private static readonly string _cacheKeys = "cache-keys";

    public CacheService(HybridCache cache)
    {
        _cache = cache;
    }

    [Obsolete("Use GetOrCreateAsync instead")]
    public ValueTask<T> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
    {
        return _cache.GetOrCreateAsync<T>(key.Key, factory: _ => default, cancellationToken: cancellationToken);
    }

    public async Task<T> GetOrCreateAsync<T>(CacheKey key, Func<Task<T>> factory, CacheOptions cacheOptions, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = cacheOptions.Expiration,
        };

        return await _cache.GetOrCreateAsync(key.Key, async _ => await factory(), options, cacheOptions.Tags, cancellationToken);
    }

    public async Task SetAsync<T>(CacheKey key, T value, CacheOptions cacheOptions, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = cacheOptions.Expiration,
        };

        await _cache.SetAsync(key.Key, value, options, cacheOptions.Tags, cancellationToken);

        await AddKeyAsync(key, cancellationToken);
    }

    public ValueTask DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key.Key, cancellationToken);
    }

    public async Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        var keys = await GetCacheKeys(options, cancellationToken);

        foreach (var key in keys.Where(k => k.StartsWith(prefix)))
        {
            keys.Remove(key);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        await _cache.RemoveByTagAsync(prefix, cancellationToken);

        await _cache.SetAsync(_cacheKeys, keys, options, [_cacheKeys], cancellationToken);
    }

    private async Task AddKeyAsync(CacheKey key, CancellationToken cancellationToken)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        var keys = await GetCacheKeys(options, cancellationToken);

        if (keys.Contains(key.Key))
        {
            return;
        }

        keys.Add(key.Key);

        await _cache.SetAsync(_cacheKeys, keys, options, [_cacheKeys], cancellationToken);
    }

    private ValueTask<List<string>> GetCacheKeys(HybridCacheEntryOptions options, CancellationToken cancellationToken)
    {
        return _cache.GetOrCreateAsync(
            _cacheKeys,
            _ => ValueTask.FromResult(new List<string>()),
            options,
            [_cacheKeys],
            cancellationToken
        );
    }
}

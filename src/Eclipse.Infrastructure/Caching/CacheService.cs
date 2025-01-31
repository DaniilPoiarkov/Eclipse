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

    public async Task<T> GetOrCreateAsync<T>(CacheKey key, Func<Task<T>> factory, CacheOptions? cacheOptions = null, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = cacheOptions?.Expiration,
        };

        var value = await _cache.GetOrCreateAsync(
            key.Key,
            async _ => await factory(),
            options,
            cacheOptions?.Tags,
            cancellationToken
        );

        return value ?? await factory();
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

    public async Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key.Key, cancellationToken);
    }

    public async Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveByTagAsync(prefix, cancellationToken);
    }

    public async Task PruneAsync(CancellationToken cancellationToken = default)
    {
        var keys = await GetCacheKeys(cancellationToken: cancellationToken);

        foreach (var key in keys)
        {
            await DeleteAsync(key, cancellationToken);
        }
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

    private ValueTask<List<string>> GetCacheKeys(HybridCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
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

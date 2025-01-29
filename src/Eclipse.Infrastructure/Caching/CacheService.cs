using Eclipse.Common.Caching;

using Microsoft.Extensions.Caching.Distributed;

using Newtonsoft.Json;

namespace Eclipse.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
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
            ContractResolver = PrivateMembersContractResolver.Instance,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        });
    }

    public async Task SetAsync<T>(CacheKey key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        await _cache.SetStringAsync(key.Key, json, options, cancellationToken);
        await AddKeyAsync(key, cancellationToken);
    }

    public Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key.Key, cancellationToken);
    }

    public async Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey("cache-keys");
        var keys = await GetAsync<List<string>>(key, cancellationToken) ?? [];

        var removing = keys
            .Where(k => k.StartsWith(prefix))
            .Select(k => DeleteAsync(k, cancellationToken));

        await Task.WhenAll(removing);
    }

    private async Task AddKeyAsync(CacheKey cacheKey, CancellationToken cancellationToken)
    {
        var key = new CacheKey("cache-keys");
        var keys = await GetAsync<List<string>>(key, cancellationToken) ?? [];

        if (keys.Contains(cacheKey.Key))
        {
            return;
        }

        keys.Add(cacheKey.Key);

        await SetAsync(key, keys, CacheConsts.ThreeDays, cancellationToken);
    }

    public async Task PruneAsync(CancellationToken cancellationToken = default)
    {
        var keys = await GetAsync<List<string>>("cache-keys") ?? [];

        foreach (var key in keys)
        {
            await DeleteAsync(key, cancellationToken);
        }
    }
}

using Eclipse.Common.Caching;

using Microsoft.Extensions.Caching.Hybrid;

using Newtonsoft.Json;

namespace Eclipse.Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly HybridCache _cache;

    private static readonly string _cacheKeys = "cache-keys";

    public CacheService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default)
    {
        var json = await _cache.GetOrCreateAsync<string>(key.Key, factory: (_) => default, cancellationToken: cancellationToken);

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

    public async Task SetAsync<T>(CacheKey key, T value, TimeSpan expiration, IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value);

        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration,
        };

        await _cache.SetAsync(key.Key, json, options, cancellationToken: cancellationToken, tags: tags);

        await AddKeyAsync(key, cancellationToken);
    }

    public async Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key.Key, cancellationToken);
    }

    public async Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var keys = await GetAsync<List<string>>(_cacheKeys, cancellationToken) ?? [];

        var removing = keys
            .Where(k => k.StartsWith(prefix))
            .Select(k => DeleteAsync(k, cancellationToken))
            .Concat([_cache.RemoveByTagAsync(prefix, cancellationToken).AsTask()]);

        await Task.WhenAll(removing);
    }

    private async Task AddKeyAsync(CacheKey cacheKey, CancellationToken cancellationToken)
    {
        var keys = await GetAsync<List<string>>(_cacheKeys, cancellationToken) ?? [];

        if (keys.Contains(cacheKey.Key))
        {
            return;
        }

        keys.Add(cacheKey.Key);

        await SetAsync(_cacheKeys, keys, CacheConsts.ThreeDays, [], cancellationToken);
    }
}

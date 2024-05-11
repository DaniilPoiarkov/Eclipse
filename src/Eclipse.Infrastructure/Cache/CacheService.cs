using Eclipse.Common.Cache;
using Eclipse.Infrastructure.Builder;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Eclipse.Infrastructure.Cache;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private readonly IOptions<CacheOptions> _options;

    public CacheService(IDistributedCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        _options = options;
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

    public Task SetAsync<T>(CacheKey key, T value, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _options.Value.Expiration
        };

        return _cache.SetStringAsync(key.Key, json, options, cancellationToken);
    }

    public Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key.Key, cancellationToken);
    }
}

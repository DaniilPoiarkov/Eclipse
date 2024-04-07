using Eclipse.Common.Cache;
using Eclipse.Infrastructure.Builder;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Eclipse.Infrastructure.Cache;

internal sealed class AsyncCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private readonly IOptions<CacheOptions> _options;

    public AsyncCacheService(IDistributedCache cache, IOptions<CacheOptions> options)
    {
        _cache = cache;
        _options = options;
    }

    public void Delete(CacheKey key)
    {
        throw new NotImplementedException();
    }

    public void Set<T>(CacheKey key, T value)
    {
        throw new NotImplementedException();
    }

    public T? Get<T>(CacheKey key)
    {
        throw new NotImplementedException();
    }

    public T? GetAndDelete<T>(CacheKey key)
    {
        throw new NotImplementedException();
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

        return JsonConvert.DeserializeObject<T>(json);
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

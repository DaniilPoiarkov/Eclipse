using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace Eclipse.Infrastructure.Internals.Cache;

internal class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    private readonly InfrastructureOptions _options;

    public CacheService(IMemoryCache memoryCache, InfrastructureOptions options)
    {
        _memoryCache = memoryCache;
        _options = options;
    }

    public T? Get<T>(CacheKey key) => _memoryCache.Get<T>(key.Key);

    public T? GetAndDelete<T>(CacheKey key)
    {
        var data = _memoryCache.Get<T>(key.Key);
        _memoryCache.Remove(key.Key);
        
        return data;
    }

    public void Set<T>(CacheKey key, T value)
    {
        _memoryCache.Set(key.Key, value, _options.CacheOptions.Expiration);
    }
}

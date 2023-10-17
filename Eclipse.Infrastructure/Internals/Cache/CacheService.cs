using Eclipse.Infrastructure.Builder;
using Eclipse.Infrastructure.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Eclipse.Infrastructure.Internals.Cache;

internal class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    private readonly IOptions<CacheOptions> _options;

    public CacheService(IMemoryCache memoryCache, IOptions<CacheOptions> options)
    {
        _memoryCache = memoryCache;
        _options = options;
    }

    public void Delete(CacheKey key)
    {
        _memoryCache.Remove(key.Key);
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
        _memoryCache.Set(key.Key, value, _options.Value.Expiration);
    }
}

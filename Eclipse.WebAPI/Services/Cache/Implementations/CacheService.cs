using Microsoft.Extensions.Caching.Memory;

namespace Eclipse.WebAPI.Services.Cache.Implementations;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(CacheKey key, bool delete = false)
    {
        var data = _memoryCache.Get<T>(key.Key);

        if (delete)
        {
            _memoryCache.Remove(key.Key);
        }

        return data;
    }

    public void Set<T>(CacheKey key, T value)
    {
        _memoryCache.Set(key.Key, value, new TimeSpan(3, 0, 0, 0, 0));
    }
}

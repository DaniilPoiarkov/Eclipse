namespace Eclipse.Infrastructure.Cache;

/// <summary>
/// Simple wrapper around memory cache with easy to use API
/// </summary>
public interface ICacheService
{
    void Set<T>(CacheKey key, T value);

    T? Get<T>(CacheKey key);

    T? GetAndDelete<T>(CacheKey key);

    void Delete(CacheKey key);
}

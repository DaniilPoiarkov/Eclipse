namespace Eclipse.Infrastructure.Cache;

public interface ICacheService
{
    void Set<T>(CacheKey key, T value);

    T? Get<T>(CacheKey key);

    T? GetAndDelete<T>(CacheKey key);
}

namespace Eclipse.WebAPI.Services.Cache;

public interface ICacheService
{
    void Set<T>(CacheKey key, T value);

    T? Get<T>(CacheKey key, bool delete = false);
}

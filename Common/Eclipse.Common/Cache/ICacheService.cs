namespace Eclipse.Common.Cache;

/// <summary>
/// Simple wrapper around memory cache with easy to use API
/// </summary>
public interface ICacheService
{
    //void Set<T>(CacheKey key, T value);

    //T? Get<T>(CacheKey key);

    //T? GetAndDelete<T>(CacheKey key);

    //void Delete(CacheKey key);

    Task SetAsync<T>(CacheKey key, T value, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    Task<T?> GetAndDeleteAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default);
}

namespace Eclipse.Common.Cache;

/// <summary>
/// Simple wrapper around memory cache with easy to use API
/// </summary>
public interface ICacheService
{
    Task SetAsync<T>(CacheKey key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    Task<T?> GetAndDeleteAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default);
}

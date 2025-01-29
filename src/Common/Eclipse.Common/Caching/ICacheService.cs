namespace Eclipse.Common.Caching;

/// <summary>
/// Simple wrapper around memory cache with easy to use API
/// </summary>
public interface ICacheService
{
    Task SetAsync<T>(CacheKey key, T value, TimeSpan expiration, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    Task DeleteAsync(CacheKey key, CancellationToken cancellationToken = default);

    Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    Task PruneAsync(CancellationToken cancellationToken = default);
}

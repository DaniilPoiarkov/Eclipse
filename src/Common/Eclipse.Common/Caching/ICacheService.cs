namespace Eclipse.Common.Caching;

/// <summary>
/// Simple wrapper around memory cache with easy to use API
/// </summary>
public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(CacheKey key, Func<Task<T>> factory, CacheOptions options, CancellationToken cancellationToken = default);

    Task SetAsync<T>(CacheKey key, T value, CacheOptions options, CancellationToken cancellationToken = default);

    [Obsolete("Use GetOrCreateAsync instead")]
    ValueTask<T> GetAsync<T>(CacheKey key, CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(CacheKey key, CancellationToken cancellationToken = default);

    Task DeleteByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}

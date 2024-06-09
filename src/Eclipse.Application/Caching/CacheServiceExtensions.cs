using Eclipse.Common.Cache;

namespace Eclipse.Application.Caching;

public static class CacheServiceExtensions
{
    public static async Task<T> GetOrCreateAsync<T>(this ICacheService cacheService,
        CacheKey key,
        Func<Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cacheService, nameof(cacheService));
        ArgumentNullException.ThrowIfNull(factory, nameof(factory));
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        var entry = await cacheService.GetAsync<T>(key, cancellationToken);

        if (entry is not null)
        {
            return entry;
        }

        entry = await factory();

        await cacheService.SetAsync(key, entry, expiration, cancellationToken);

        return entry;
    }
}

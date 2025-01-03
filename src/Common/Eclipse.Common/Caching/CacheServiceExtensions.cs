namespace Eclipse.Common.Caching;

public static class CacheServiceExtensions
{
    public static async Task<T> GetOrCreateAsync<T>(this ICacheService service, CacheKey key, Func<Task<T>> factory, TimeSpan expiration, IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        var value = await service.GetAsync<T>(key, cancellationToken);

        if (value is not null)
        {
            return value;
        }

        value = await factory();

        await service.SetAsync(key, value, expiration, tags, cancellationToken);

        return value;
    }

    public static async Task<T?> GetAndDeleteAsync<T>(this ICacheService service, CacheKey key, CancellationToken cancellationToken = default)
    {
        var value = await service.GetAsync<T>(key, cancellationToken);

        await service.DeleteAsync(key, cancellationToken);

        return value;
    }
}

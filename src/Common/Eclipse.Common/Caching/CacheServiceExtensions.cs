namespace Eclipse.Common.Caching;

public static class CacheServiceExtensions
{
    public static async Task<T?> GetAndDeleteAsync<T>(this ICacheService service, CacheKey key, CancellationToken cancellationToken = default)
    {
        var value = await service.GetAsync<T>(key, cancellationToken);

        await service.DeleteAsync(key, cancellationToken);

        return value;
    }
}

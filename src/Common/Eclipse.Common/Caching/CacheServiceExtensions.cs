namespace Eclipse.Common.Caching;

public static class CacheServiceExtensions
{
    public static async Task<T?> GetAndDeleteAsync<T>(this ICacheService service, CacheKey key, CancellationToken cancellationToken = default)
    {
        // TODO: Review
        var value = await service.GetOrCreateAsync<T>(key, () => default!, cancellationToken: cancellationToken);

        await service.DeleteAsync(key, cancellationToken);

        return value;
    }
}

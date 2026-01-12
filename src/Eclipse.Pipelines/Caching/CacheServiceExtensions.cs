using Eclipse.Common.Caching;

namespace Eclipse.Pipelines.Caching;

internal static class CacheServiceExtensions
{
    public static Task SetForThreeDaysAsync<T>(this ICacheService service, string key, T value, long chatId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
            Tags = [$"chatid-{chatId}"]
        };

        return service.SetAsync(key, value, options, cancellationToken);
    }

    public static async Task<int> GetListPageAsync(this ICacheService service, string cacheKey, string buttonClick, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays
        };

        var page = await service.GetOrCreateAsync(cacheKey, () => Task.FromResult(1), options, cancellationToken);

        page = buttonClick switch
        {
            "◀️" => page - 1,
            "▶️" => page + 1,
            _ => page
        };

        if (page <= 0)
        {
            page = 1;
        }

        await service.SetAsync(cacheKey, page, options, cancellationToken);

        return page;
    }
}

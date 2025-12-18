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
}

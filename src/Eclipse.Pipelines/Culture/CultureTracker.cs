
using Eclipse.Common.Caching;

namespace Eclipse.Pipelines.Culture;

internal sealed class CultureTracker : ICultureTracker
{
    private readonly ICacheService _cacheService;

    public CultureTracker(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task<string?> GetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        return _cacheService.GetAsync<string>(GetKey(chatId), cancellationToken);
    }

    public Task ResetAsync(long chatId, string culture, CancellationToken cancellationToken = default)
    {
        return _cacheService.SetAsync(GetKey(chatId), culture, CacheConsts.ThreeDays, cancellationToken);
    }

    private static CacheKey GetKey(long chatId) => new($"lang-{chatId}");
}

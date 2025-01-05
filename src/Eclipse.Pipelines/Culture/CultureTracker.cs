
using Eclipse.Common.Caching;

namespace Eclipse.Pipelines.Culture;

// TODO: Review.
internal sealed class CultureTracker : ICultureTracker
{
    private readonly ICacheService _cacheService;

    public CultureTracker(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async ValueTask<string> GetAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        return await _cacheService.GetOrCreateAsync(GetKey(chatId), () => Task.FromResult(string.Empty), options, cancellationToken);
    }

    public Task ResetAsync(long chatId, string culture, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays,
        };

        return _cacheService.SetAsync(GetKey(chatId), culture, options, cancellationToken);
    }

    private static string GetKey(long chatId) => $"lang-{chatId}";
}

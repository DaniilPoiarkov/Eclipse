using Eclipse.Common.Caching;

namespace Eclipse.Pipelines.Stores;

internal abstract class StoreBase<TObject, TKey> : IStore<TObject, TKey>
    where TKey : StoreKey
{
    private readonly ICacheService _cacheService;

    protected StoreBase(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    // TODO: Review
    public virtual async Task<TObject?> GetOrDefaultAsync(TKey key, CancellationToken cancellationToken = default)
    {
        return await _cacheService.GetOrCreateAsync(key.ToCacheKey(), () => Task.FromResult<TObject?>(default), cancellationToken: cancellationToken);
    }

    public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        return _cacheService.DeleteAsync(key.ToCacheKey(), cancellationToken);
    }

    public virtual Task SetAsync(TKey key, TObject value, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays
        };

        return _cacheService.SetAsync(key.ToCacheKey(), value, options, cancellationToken);
    }
}

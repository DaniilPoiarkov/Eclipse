using Eclipse.Common.Cache;

namespace Eclipse.Pipelines.Stores;

internal abstract class StoreBase<TObject, TKey> : IStore<TObject, TKey>
    where TKey : StoreKey
{
    private readonly ICacheService _cacheService;

    protected StoreBase(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public virtual Task<TObject?> GetOrDefaultAsync(TKey key, CancellationToken cancellationToken = default)
    {
        return _cacheService.GetAsync<TObject>(key.ToCacheKey(), cancellationToken);
    }

    public virtual Task RemoveAsync(TKey key, CancellationToken cancellationToken = default)
    {
        return _cacheService.DeleteAsync(key.ToCacheKey(), cancellationToken);
    }

    public virtual Task SetAsync(TKey key, TObject value, CancellationToken cancellationToken = default)
    {
        return _cacheService.SetAsync(key.ToCacheKey(), value, cancellationToken);
    }
}

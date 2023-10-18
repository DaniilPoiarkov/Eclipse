using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Stores;

internal abstract class StoreBase<TObject, TKey> : IStore<TObject, TKey>
    where TKey : StoreKey
{
    private readonly ICacheService _cacheService;

    protected StoreBase(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public virtual TObject? GetOrDefault(TKey key)
    {
        return _cacheService.Get<TObject>(key.ToCacheKey());
    }

    public virtual void Remove(TKey key)
    {
        _cacheService.Delete(key.ToCacheKey());
    }

    public virtual void Set(TKey key, TObject value)
    {
        _cacheService.Set(key.ToCacheKey(), value);
    }
}

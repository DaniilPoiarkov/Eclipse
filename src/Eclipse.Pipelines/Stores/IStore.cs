namespace Eclipse.Pipelines.Stores;

public interface IStore<TObject, TKey>
{
    Task<TObject?> GetOrDefaultAsync(TKey key, CancellationToken cancellationToken = default);

    Task SetAsync(TKey key, TObject value, CancellationToken cancellationToken = default);

    Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}

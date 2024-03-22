namespace Eclipse.Pipelines.Stores;

public interface IStore<TObject, TKey>
{
    TObject? GetOrDefault(TKey key);

    void Set(TKey key, TObject value);

    void Remove(TKey key);
}

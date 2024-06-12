namespace Eclipse.Common.Cache;

public class CacheKey
{
    public string Key { get; }

    public CacheKey(string key)
    {
        Key = key;
    }

    public static implicit operator CacheKey(string key) => new(key);
}

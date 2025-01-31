namespace Eclipse.Common.Caching;

public sealed class CacheOptions
{
    public IEnumerable<string> Tags { get; set; } = [];

    public TimeSpan Expiration { get; set; }
}

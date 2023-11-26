namespace Eclipse.Infrastructure.Builder;

public class CacheOptions
{
    public TimeSpan Expiration { get; set; } = new TimeSpan(3, 0, 0, 0);
}

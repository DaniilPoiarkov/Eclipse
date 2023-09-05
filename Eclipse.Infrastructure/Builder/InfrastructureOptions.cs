namespace Eclipse.Infrastructure.Builder;

public class InfrastructureOptions
{
    public TelegramOptions TelegramOptions { get; }

    public CacheOptions CacheOptions { get; }

    public InfrastructureOptions(TelegramOptions telegramOptions, CacheOptions cacheOptions)
    {
        TelegramOptions = telegramOptions;
        CacheOptions = cacheOptions;
    }
}


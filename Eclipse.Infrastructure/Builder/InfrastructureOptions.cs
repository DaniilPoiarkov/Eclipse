namespace Eclipse.Infrastructure.Builder;

internal class InfrastructureOptions
{
    internal TelegramOptions TelegramOptions { get; }

    public CacheOptions CacheOptions { get; }

    public InfrastructureOptions(TelegramOptions telegramOptions, CacheOptions cacheOptions)
    {
        TelegramOptions = telegramOptions;
        CacheOptions = cacheOptions;
    }
}


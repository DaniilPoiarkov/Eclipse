namespace Eclipse.Infrastructure.Builder;

public class InfrastructureOptions
{
    public TelegramOptions TelegramOptions { get; }

    public CacheOptions CacheOptions { get; }

    public GoogleOptions GoogleOptions { get; }

    public InfrastructureOptions(
        TelegramOptions telegramOptions,
        CacheOptions cacheOptions,
        GoogleOptions googleOptions)
    {
        TelegramOptions = telegramOptions;
        CacheOptions = cacheOptions;
        GoogleOptions = googleOptions;
    }
}


using Eclipse.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;

namespace Eclipse.Infrastructure.Builder;

public class InfrastructureOptionsBuilder
{
    private readonly IServiceCollection _services;

    public TelegramOptions? TelegramOptions { get; set; }

    public CacheOptions? CacheOptions { get; set; }

    public InfrastructureOptionsBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void UseTelegramHandler<THandler>()
        where THandler : IUpdateHandler =>
        _services.AddTransient<IUpdateHandler>(sp => sp.GetRequiredService<THandler>());

    internal InfrastructureOptions Build()
    {
        if (TelegramOptions is null || string.IsNullOrEmpty(TelegramOptions.Token))
        {
            throw new InfrastructureException("Telegram options are invalid");
        }

        CacheOptions ??= new();

        return new InfrastructureOptions(TelegramOptions, CacheOptions);
    }
}

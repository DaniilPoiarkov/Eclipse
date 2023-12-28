using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Polling;

namespace Eclipse.Infrastructure.Builder;

internal sealed class InfrastructureModuleBuilder : IInfrastructureModuleBuilder
{
    private readonly IServiceCollection _services;

    public InfrastructureModuleBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IInfrastructureModuleBuilder UseTelegramHandler<THandler>()
        where THandler : IUpdateHandler
    {
        _services.AddScoped<IUpdateHandler>(sp => sp.GetRequiredService<THandler>());
        return this;
    }

    public IInfrastructureModuleBuilder ConfigureTelegramOptions(Action<TelegramOptions> options)
        => Configure(options);

    public IInfrastructureModuleBuilder ConfigureGoogleOptions(Action<GoogleOptions> options)
        => Configure(options);

    public IInfrastructureModuleBuilder ConfigureCacheOptions(Action<CacheOptions> options)
        => Configure(options);

    private InfrastructureModuleBuilder Configure<TOptions>(Action<TOptions> options)
        where TOptions : class
    {
        _services.Configure(options);
        return this;
    }
}

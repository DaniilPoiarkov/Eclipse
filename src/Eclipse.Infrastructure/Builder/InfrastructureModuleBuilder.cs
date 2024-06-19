using Eclipse.Common.Telegram;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Infrastructure.Builder;

internal sealed class InfrastructureModuleBuilder : IInfrastructureModuleBuilder
{
    private readonly IServiceCollection _services;

    public InfrastructureModuleBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public IInfrastructureModuleBuilder ConfigureTelegramOptions(Action<TelegramOptions> options)
        => Configure(options);

    public IInfrastructureModuleBuilder ConfigureGoogleOptions(Action<GoogleOptions> options)
        => Configure(options);

    private InfrastructureModuleBuilder Configure<TOptions>(Action<TOptions> options)
        where TOptions : class
    {
        _services.Configure(options);
        return this;
    }
}

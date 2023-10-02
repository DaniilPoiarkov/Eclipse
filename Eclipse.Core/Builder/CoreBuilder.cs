using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Builder;

public class CoreBuilder
{
    private readonly IServiceCollection _services;

    public CoreBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public CoreBuilder Decorate<TCoreDecorator>()
        where TCoreDecorator : class, ICoreDecorator
    {
        _services.AddTransient<ICoreDecorator, TCoreDecorator>();
        return this;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Builder;

public class CoreBuilder
{
    private readonly IServiceCollection _services;

    public CoreBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public CoreBuilder Decorate<TPipelineExecutionDecorator>()
        where TPipelineExecutionDecorator : class, IPipelineExecutionDecorator
    {
        _services.AddTransient<IPipelineExecutionDecorator, TPipelineExecutionDecorator>();
        return this;
    }
}

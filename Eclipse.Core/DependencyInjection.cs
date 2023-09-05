using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddEclipseCore(this IServiceCollection services)
    {
        services.AddTransient<INotFoundPipeline, NotFoundPipeline>()
            .AddTransient<IAccessDeniedPipeline, AccessDeniedPipeline>()
            .AddTransient<IPipelineProvider, PipelineProvider>();

        return services;
    }
}

using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core;

/// <summary>
/// Takes responsibility to provide useful API to implement pipelines
/// </summary>
public static class EclipseCoreModule
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services)
    {
        services.AddTransient<INotFoundPipeline, NotFoundPipeline>()
            .AddTransient<IAccessDeniedPipeline, AccessDeniedPipeline>()
            .AddTransient<IPipelineProvider, PipelineProvider>();

        return services;
    }
}

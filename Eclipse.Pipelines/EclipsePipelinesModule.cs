using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.UpdateHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Pipelines;

/// <summary>
/// Takes responsibility for pipelines registration and implementation
/// </summary>
public static class EclipsePipelinesModule
{
    public static IServiceCollection AddPipelinesModule(this IServiceCollection services)
    {
        services
            .Replace(ServiceDescriptor.Transient<INotFoundPipeline, EclipseNotFoundPipeline>())
            .AddTransient<ITelegramUpdateHandler, TelegramUpdateHandler>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>())
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());
        
        return services;
    }
}

using Eclipse.Core.Builder;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.UpdateParsing;
using Eclipse.Core.UpdateParsing.Implementations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Core;

/// <summary>
/// Takes responsibility to provide useful API to implement pipelines
/// </summary>
public static class EclipseCoreModule
{
    public static IServiceCollection AddCoreModule(this IServiceCollection services, Action<CoreBuilder>? builder = null)
    {
        var coreBuilder = new CoreBuilder(services);

        builder?.Invoke(coreBuilder);

        services
            .AddTransient<ILegacyPipelineProvider, LegacyPipelineProvider>()
            .AddTransient<IUpdateParser, UpdateParser>()
            .AddTransient<IParseStrategyProvider, ParseStrategyProvider>()
                .AddScoped<IPipelineProvider, PipelineProvider>()
                .AddScoped<IPipelineExecutionDecorator, NullPipelineExecutionDecorator>();

        services.Scan(tss => tss.FromAssemblyOf<IUpdateParser>()
            .AddClasses(c => c.AssignableTo<IParseStrategy>(), publicOnly: false)
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<PipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>(), publicOnly: false)
            .As<PipelineBase>()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<IProviderHandler>()
            .AddClasses(c => c.AssignableTo<IProviderHandler>(), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.TryAddSingleton<IKeywordMapper, NullKeywordMapper>();

        return services;
    }
}

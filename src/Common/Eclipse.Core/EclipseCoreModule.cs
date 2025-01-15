using Eclipse.Core.Builder;
using Eclipse.Core.Core;
using Eclipse.Core.CurrentUser;
using Eclipse.Core.Pipelines;
using Eclipse.Core.UpdateParsing;
using Eclipse.Core.UpdateParsing.Implementations;

using Microsoft.Extensions.DependencyInjection;

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
            .AddTransient<IPipelineProvider, PipelineProvider>()
            .AddTransient<IUpdateParser, UpdateParser>()
            .AddTransient<IParseStrategyProvider, ParseStrategyProvider>()
                .AddScoped<ICurrentTelegramUser, CurrentTelegramUser>()
                .AddScoped<IPipelineExecutionDecorator, NullPipelineExecutionDecorator>()
                .AddScoped<IUpdateProvider, UpdateProvider>();

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

        return services;
    }
}

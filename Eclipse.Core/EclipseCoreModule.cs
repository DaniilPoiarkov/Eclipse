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
    public static IServiceCollection AddCoreModule(this IServiceCollection services)
    {
        services.AddTransient<INotFoundPipeline, NotFoundPipeline>()
            .AddTransient<IAccessDeniedPipeline, AccessDeniedPipeline>()
            .AddTransient<IPipelineProvider, PipelineProvider>()
            .AddTransient<IUpdateParser, UpdateParser>()
            .AddTransient<IParseStrategyProvider, ParseStrategyProvider>()
            .AddScoped<ICurrentTelegramUser, CurrentTelegramUser>();

        services.Scan(tss => tss.FromAssemblyOf<IUpdateParser>()
            .AddClasses(c => c.AssignableTo<IParseStrategy>())
            .AsSelf()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}

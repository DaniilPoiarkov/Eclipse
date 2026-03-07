using Eclipse.Core.Builder;
using Eclipse.Core.Keywords;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Provider.Handlers;
using Eclipse.Core.Stores;
using Eclipse.Core.UpdateParsing;
using Eclipse.Core.UpdateParsing.Implementations;
using Eclipse.Core.UpdateParsing.Strategies;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Eclipse.Core;

/// <summary>
/// Takes responsibility to provide useful API to implement pipelines
/// </summary>
public static class EclipseCoreModule
{
    private const int DefaultMessagesPersistanceInDays = 3;

    public static IServiceCollection AddCoreModule(this IServiceCollection services, Action<CoreBuilder>? builder = null)
    {
        var coreBuilder = new CoreBuilder(services);

        builder?.Invoke(coreBuilder);

        services
            .AddScoped<IPipelineProvider, PipelineProvider>()
            .AddScoped<IPipelineExecutionDecorator, NullPipelineExecutionDecorator>();

        services
            .AddTransient<IUpdateParser, UpdateParser>()
            .AddTransient<IParseStrategyProvider, ParseStrategyProvider>()
            .AddTransient<IParseStrategy, CallbackQueryParseStrategy>()
            .AddTransient<IParseStrategy, MessageParseStrategy>()
            .AddTransient<IParseStrategy, UnknownParseStrategy>()
            .AddTransient<IParseStrategy, MyChatMemberParseStrategy>();

        services
            .AddScoped<IRouteHandler, MessageHandler>()
            .AddScoped<IRouteHandler, MyChatMemberHandler>()
            .AddScoped<IRouteHandler, UnknownHandler>();

        if (!AreStoresRegistered(services))
        {
            throw new ArgumentException("Stores are not configured. Call CoreBuilder.UseInMemoryStores(); if no other provider is registered.", nameof(services));
        }

        if (!services.Any(s => s.ServiceType == typeof(IOptions<CoreOptions>)))
        {
            services.Configure<CoreOptions>(options =>
            {
                options.MessagePersistanceInDays = DefaultMessagesPersistanceInDays;
            });
        }

        services.TryAddSingleton<IKeywordMapper, NullKeywordMapper>();
        services.TryAddSingleton<INotFoundPipeline, NotFoundPipeline>();
        services.TryAddScoped<IAccessDeniedPipeline, AccessDeniedPipeline>();

        return services;
    }

    private static bool AreStoresRegistered(this IServiceCollection services)
    {
        return services.Any(s => s.ServiceType == typeof(IPipelineStore))
            && services.Any(s => s.ServiceType == typeof(IMessageStore));
    }
}

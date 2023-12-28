using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Configurations;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.Pipelines.Users;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Builder;

using Serilog;

using Telegram.Bot.Polling;
using Telegram.Bot;

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
            .Replace(ServiceDescriptor.Transient<IAccessDeniedPipeline, EclipseAccessDeniedPipeline>())
                .AddTransient<IEclipseUpdateHandler, EclipseUpdateHandler>()
                .AddTransient<IUserStore, UserStore>()
                .AddTransient<IMessageStore, MessageStore>()
                .AddTransient<IPipelineStore, PipelineStore>()
            .AddSingleton<ITelegramUpdateHandler, TelegramUpdateHandler>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>())
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses(c => c.AssignableTo<EclipseJobBase>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.ConfigureOptions<QuatzOptionsConfiguration>();
        
        return services;
    }

    public static async Task InitializePipelineModuleAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<ILogger>();
        var client = serviceProvider.GetRequiredService<ITelegramBotClient>();
        var updateHandler = serviceProvider.GetRequiredService<IUpdateHandler>();

        logger.Information("Initializing {module} module", nameof(EclipsePipelinesModule));
        client.StartReceiving(updateHandler);

        var me = await client.GetMeAsync();

        logger.Information("\tBot: {bot}", me.Username);
        logger.Information("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }
}

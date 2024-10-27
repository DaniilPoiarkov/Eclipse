using Eclipse.Application.Contracts.Url;
using Eclipse.Common.Background;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Configurations;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Health;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.UpdateHandler;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Telegram.Bot;

namespace Eclipse.Pipelines;

/// <summary>
/// Takes responsibility for pipelines registration and implementation
/// </summary>
public static class EclipsePipelinesModule
{
    public static IServiceCollection AddPipelinesModule(this IServiceCollection services)
    {
        services.RemoveAll<PipelineBase>();

        services
            .AddTransient<IEclipseUpdateHandler, EclipseUpdateHandler>()
            .AddTransient<IEclipseUpdateHandler, DisabledUpdateHandler>()
            .AddTransient<IMessageStore, MessageStore>()
            .AddTransient<IPipelineStore, PipelineStore>()
            .AddTransient<ICultureTracker, CultureTracker>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>())
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses(c => c.AssignableTo<EclipseJobBase>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)))
            .AsSelf()
            .WithTransientLifetime());

        services.ConfigureOptions<QuatzOptionsConfiguration>();

        var configuration = services.GetConfiguration();

        services.AddPipelinesHealthChecks();

        return services;
    }

    public static async Task InitializePipelineModuleAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<ILogger<TelegramBotClient>>();
        var client = serviceProvider.GetRequiredService<ITelegramBotClient>();

        logger.LogInformation("Initializing {module} module", nameof(EclipsePipelinesModule));

        await ResetWebhookAsync(serviceProvider, client);

        var me = await client.GetMeAsync();

        logger.LogInformation("\tBot: {bot}", me?.Username);
        logger.LogInformation("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }

    private static async Task ResetWebhookAsync(IServiceProvider serviceProvider, ITelegramBotClient client)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var appUrlProvider = serviceProvider.GetRequiredService<IAppUrlProvider>();

        var webhookInfo = await client.GetWebhookInfoAsync();

        var endpoint = configuration["Telegram:ActiveEndpoint"]!;

        var webhook = $"{appUrlProvider.AppUrl.EnsureEndsWith('/')}{endpoint}";

        if (webhookInfo is not null && webhookInfo.Url.Equals(webhook))
        {
            return;
        }

        await client.SetWebhookAsync(
            url: webhook,
            secretToken: configuration["Telegram:SecretToken"]
        );
    }
}

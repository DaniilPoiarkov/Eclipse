﻿using Eclipse.Application.Contracts.Url;
using Eclipse.Common.Background;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Configurations;
using Eclipse.Pipelines.Health;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.Options.Languages;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.UpdateHandler;
using Eclipse.Pipelines.Users;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        services
            .Replace(ServiceDescriptor.Transient<INotFoundPipeline, EclipseNotFoundPipeline>())
            .Replace(ServiceDescriptor.Transient<IAccessDeniedPipeline, EclipseAccessDeniedPipeline>())
                .AddTransient<IEclipseUpdateHandler, EclipseUpdateHandler>()
                .AddTransient<IEclipseUpdateHandler, DisabledUpdateHandler>()
                .AddTransient<IUserStore, UserStore>()
                .AddTransient<IMessageStore, MessageStore>()
                .AddTransient<IPipelineStore, PipelineStore>();

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

        services.Configure<LanguageList>(
            configuration.GetSection(nameof(LanguageList))
        );

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

        var webhook = $"{appUrlProvider.AppUrl}/{endpoint}";

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

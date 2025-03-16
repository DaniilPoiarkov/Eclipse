using Eclipse.Application.Contracts.Url;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Background;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Health;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.UpdateHandler;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Telegram.Bot;

namespace Eclipse.Pipelines;

/// <summary>
/// Takes responsibility for pipelines registration and implementation
/// </summary>
public static class EclipsePipelinesModule
{
    private static readonly int _retriesCount = 5;

    private static readonly int _baseRetryDelay = 1;

    public static IServiceCollection AddPipelinesModule(this IServiceCollection services, Action<PipelinesOptions> options)
    {
        services.Configure(options);

        services.RemoveAll<PipelineBase>();

        services
            .AddTransient<IEclipseUpdateHandler, EclipseUpdateHandler>()
            .AddTransient<IEclipseUpdateHandler, DisabledUpdateHandler>()
            .AddTransient<IMessageStore, MessageStore>()
            .AddTransient<IPipelineStore, PipelineStore>()
            .AddTransient<ICultureTracker, CultureTracker>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>(), publicOnly: false)
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses(c => c.AssignableTo<EclipseJobBase>(), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services
            .AddPipelinesHealthChecks()
            .AddTelegramIntegration();

        services.AddTransient<IMoodRecordCollector, MoodRecordCollector>();

        return services;
    }

    private static IServiceCollection AddTelegramIntegration(this IServiceCollection services)
    {
        services.AddHttpClient<ITelegramBotClient, TelegramBotClient>((client, sp) =>
        {
            var options = sp.GetRequiredService<IOptions<PipelinesOptions>>().Value;
            return new TelegramBotClient(options.Token, client);
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(_baseRetryDelay), _retriesCount)
            ));

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

        var me = await client.GetMe();

        logger.LogInformation("\tBot: {bot}", me?.Username);
        logger.LogInformation("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }

    private static async Task ResetWebhookAsync(IServiceProvider serviceProvider, ITelegramBotClient client)
    {
        var options = serviceProvider.GetRequiredService<IOptions<PipelinesOptions>>();
        var appUrlProvider = serviceProvider.GetRequiredService<IAppUrlProvider>();

        var webhookInfo = await client.GetWebhookInfo();

        var webhook = $"{appUrlProvider.AppUrl.EnsureEndsWith('/')}{options.Value.ActiveEndpoint}";

        if (webhookInfo is not null && webhookInfo.Url.Equals(webhook))
        {
            return;
        }

        await client.SetWebhook(
            url: webhook,
            secretToken: options.Value.SecretToken
        );
    }
}

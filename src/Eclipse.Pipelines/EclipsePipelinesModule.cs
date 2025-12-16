using Eclipse.Application.Contracts.Url;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Background;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Culture;
using Eclipse.Pipelines.Feedbacks;
using Eclipse.Pipelines.Health;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.MoodRecords;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.AdminMenu.Export.Jobs;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.Stores.Users;
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
            .AddTransient<IUserStore, UserStore>()
            .AddTransient<ICultureTracker, CultureTracker>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>(), publicOnly: false)
            .As<PipelineBase>()
            .AsSelf()
            .WithScopedLifetime());

        services.Scan(tss => tss.FromAssemblyOf<ExportToUserBackgroundJobArgs>()
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services
            .AddPipelinesHealthChecks()
            .AddTelegramIntegration();

        services.AddTransient<IMoodRecordCollector, MoodRecordCollector>()
            .AddTransient<IFeedbackCollector, FeedbackCollector>();

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

    public static async Task InitializePipelineModuleAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var serviceProvider = scope.ServiceProvider;

        var logger = serviceProvider.GetRequiredService<ILogger<TelegramBotClient>>();
        var client = serviceProvider.GetRequiredService<ITelegramBotClient>();

        logger.LogInformation("Initializing {module} module", nameof(EclipsePipelinesModule));

        await ResetWebhookAsync(serviceProvider, client, cancellationToken);

        var me = await client.GetMe(cancellationToken);

        logger.LogInformation("\tBot: {bot}", me?.Username);
        logger.LogInformation("{module} module initialized successfully", nameof(EclipsePipelinesModule));
    }

    private static async Task ResetWebhookAsync(IServiceProvider serviceProvider, ITelegramBotClient client, CancellationToken cancellationToken)
    {
        var options = serviceProvider.GetRequiredService<IOptions<PipelinesOptions>>();
        var appUrlProvider = serviceProvider.GetRequiredService<IAppUrlProvider>();

        var webhookInfo = await client.GetWebhookInfo(cancellationToken);

        var webhook = $"{appUrlProvider.AppUrl.EnsureEndsWith('/')}{options.Value.ActiveEndpoint}";

        if (webhookInfo is not null && webhookInfo.Url.Equals(webhook))
        {
            return;
        }

        await client.SetWebhook(
            url: webhook,
            secretToken: options.Value.SecretToken,
            cancellationToken: cancellationToken
        );
    }
}

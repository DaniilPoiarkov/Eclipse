using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Infrastructure.Builder;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.Users;

using Microsoft.Extensions.Options;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal sealed class MorningJob : EclipseJobBase
{
    private static readonly TimeOnly Morning = new(9, 0);

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IUserStore _identityUserStore;

    private readonly IOptions<TelegramOptions> _options;

    private readonly IServiceProvider _serviceProvider;

    public MorningJob(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IUserStore identityUserStore,
        IOptions<TelegramOptions> options,
        IServiceProvider serviceProvider)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _identityUserStore = identityUserStore;
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.UtcNow.GetTime();

        var users = _identityUserStore.GetCachedUsers()
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == Morning)
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        // TODO: Remove after fixing job
        await _botClient.SendTextMessageAsync(
            _options.Value.Chat,
            $"Sending morning message to {users.Count} user(s):\n\r{string.Join("\n\r * @", users.Select(u => u.Username))}",
            cancellationToken: context.CancellationToken);

        var notifications = new List<Task>(users.Count);

        foreach (var user in users)
        {
            try
            {
                var key = new PipelineKey(user.ChatId);
                _pipelineStore.Remove(key);

                var pipeline = _pipelineProvider.Get("/daily_morning");

                await _botClient.SendTextMessageAsync(
                    _options.Value.Chat,
                    $"Morning job {pipeline.GetType().Name} pipeline found",
                    cancellationToken: context.CancellationToken);

                var messageContext = new MessageContext(
                    user.ChatId,
                    string.Empty,
                    new TelegramUser(user.ChatId, user.Name, user.Surname, user.Username),
                    _serviceProvider
                );

                var result = await pipeline.RunNext(messageContext, context.CancellationToken);

                notifications.Add(result.SendAsync(_botClient, context.CancellationToken));

                _pipelineStore.Set(key, pipeline);
            }
            catch (Exception ex)
            {
                await _botClient.SendTextMessageAsync(
                    _options.Value.Chat,
                    $"Exception in morning job:\n\r\n\r{ex}",
                    cancellationToken: context.CancellationToken);
            }
        }

        try
        {
            await Task.WhenAll(notifications);
        }
        catch (Exception ex)
        {
            await _botClient.SendTextMessageAsync(
                _options.Value.Chat,
                $"Exception in await all:\n\r\n\r{ex}",
                cancellationToken: context.CancellationToken);
        }
    }
}

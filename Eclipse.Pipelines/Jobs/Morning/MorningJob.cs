using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.Users;

using Quartz;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs.Morning;

internal class MorningJob : EclipseJobBase
{
    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IUserStore _identityUserStore;

    public MorningJob(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IUserStore identityUserStore)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _identityUserStore = identityUserStore;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var users = _identityUserStore.GetCachedUsers()
            .Where(u => u.NotificationsEnabled)
            .ToList();

        var notifications = new List<Task>(users.Count);

        foreach (var user in users)
        {
            var key = new PipelineKey(user.ChatId);
            _pipelineStore.Remove(key);

            var pipeline = _pipelineProvider.Get("/daily_morning");

            var messageContext = new MessageContext(user.ChatId, string.Empty, new TelegramUser(user.ChatId, user.Name, user.Surname, user.Username));

            var result = await pipeline.RunNext(messageContext, context.CancellationToken);

            notifications.Add(result.SendAsync(_botClient, context.CancellationToken));

            _pipelineStore.Set(key, pipeline);
        }

        await Task.WhenAll(notifications);
    }
}

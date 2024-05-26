using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.Users;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.Morning;

internal sealed class MorningJob : EclipseJobBase
{
    private static readonly TimeOnly Morning = new(9, 0);

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IUserStore _userStore;

    private readonly IServiceProvider _serviceProvider;

    private readonly IEclipseLocalizer _localizer;

    private readonly IMessageStore _messageStore;

    public MorningJob(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IUserStore userStore,
        IServiceProvider serviceProvider,
        IEclipseLocalizer localizer,
        IMessageStore messageStore)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _userStore = userStore;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
        _messageStore = messageStore;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.UtcNow.GetTime();

        var users = (await _userStore.GetCachedUsersAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == Morning)
            .ToList();

        if (users.IsNullOrEmpty())
        {
            return;
        }

        var notifications = new List<Task<Message?>>(users.Count);

        foreach (var user in users)
        {

            var key = new PipelineKey(user.ChatId);
            await _pipelineStore.RemoveAsync(key, context.CancellationToken);

            var pipeline = (_pipelineProvider.Get("/daily_morning") as EclipsePipelineBase)!;

            await _localizer.ResetCultureForUserWithChatIdAsync(user.ChatId, context.CancellationToken);

            pipeline.SetLocalizer(_localizer);

            var messageContext = new MessageContext(
                user.ChatId,
                string.Empty,
                new TelegramUser(user.ChatId, user.Name, user.Surname, user.UserName),
                _serviceProvider
            );

            var result = await pipeline.RunNext(messageContext, context.CancellationToken);

            notifications.Add(result.SendAsync(_botClient, context.CancellationToken));

            await _pipelineStore.SetAsync(key, pipeline, context.CancellationToken);
        }

        var messages = await Task.WhenAll(notifications);

        foreach (var message in messages)
        {
            if (message is null)
            {
                continue;
            }

            await _messageStore.SetAsync(new MessageKey(message.Chat.Id), message, context.CancellationToken);
        }
    }
}

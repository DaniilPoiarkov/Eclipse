using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Clock;
using Eclipse.Core.Core;
using Eclipse.Core.Models;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Extensions;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.Localization;

using Quartz;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Jobs.Evening;

internal sealed class CollectMoodRecordsJob : EclipseJobBase
{
    private static readonly TimeOnly _evening = new(19, 0);

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IServiceProvider _serviceProvider;

    private readonly IStringLocalizer<CollectMoodRecordsJob> _localizer;

    private readonly IMessageStore _messageStore;

    private readonly IUserService _userService;

    private readonly ITimeProvider _timeProvider;

    public CollectMoodRecordsJob(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IStringLocalizer<CollectMoodRecordsJob> localizer,
        IMessageStore messageStore,
        IUserService userService,
        ITimeProvider timeProvider)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
        _messageStore = messageStore;
        _userService = userService;
        _timeProvider = timeProvider;
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        var time = _timeProvider.Now.GetTime();

        var users = (await _userService.GetAllAsync(context.CancellationToken))
            .Where(u => u.NotificationsEnabled
                && time.Add(u.Gmt) == _evening)
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

            var pipeline = (_pipelineProvider.Get("/href_mood_records_add") as EclipsePipelineBase)!;

            using var _ = _localizer.UsingCulture(user.Culture);

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

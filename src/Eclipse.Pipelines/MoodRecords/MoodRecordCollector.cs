using Eclipse.Application.Contracts.Users;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Core.Context;
using Eclipse.Core.Provider;
using Eclipse.Core.Stores;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Pipelines;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.MoodRecords;

internal sealed class MoodRecordCollector : IMoodRecordCollector
{
    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ITelegramBotClient _botClient;

    private readonly IServiceProvider _serviceProvider;

    private readonly IStringLocalizer<MoodRecordCollector> _localizer;

    private readonly IMessageStore _messageStore;

    private readonly IUserService _userService;

    private readonly ICurrentCulture _currentCulture;

    private readonly ILogger<MoodRecordCollector> _logger;

    public MoodRecordCollector(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IStringLocalizer<MoodRecordCollector> localizer,
        IMessageStore messageStore,
        IUserService userService,
        ICurrentCulture currentCulture,
        ILogger<MoodRecordCollector> logger)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
        _messageStore = messageStore;
        _userService = userService;
        _currentCulture = currentCulture;
        _logger = logger;
    }

    public async Task CollectAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;

        var update = new Update
        {
            Message = new Message
            {
                Text = "/href_mood_records_add",
                From = new User
                {
                    Id = user.ChatId,
                    FirstName = user.Name,
                    LastName = user.Surname,
                    Username = user.UserName
                }
            },
        };

        await _pipelineStore.RemoveAll(user.ChatId, cancellationToken);

        var pipeline = (EclipsePipelineBase)_pipelineProvider.Get(update);

        using var _ = _currentCulture.UsingCulture(user.Culture);

        pipeline.SetLocalizer(_localizer);

        var messageContext = new MessageContext(
            user.ChatId,
            string.Empty,
            new TelegramUser(user.ChatId, user.Name, user.Surname, user.UserName),
            _serviceProvider
        );

        var stage = await pipeline.RunNext(messageContext, cancellationToken);
        var message = await stage.SendAsync(_botClient, cancellationToken);

        if (message is null)
        {
            _logger.LogWarning("{Feature}. Sent message is null for user {UserId}", "Mood record collection", user.Id);
            await _pipelineStore.Set(user.ChatId, pipeline, cancellationToken);
            return;
        }

        await _messageStore.Set(message.Chat.Id, message, cancellationToken);
        await _pipelineStore.Set(user.ChatId, message.Id, pipeline, cancellationToken);
    }
}

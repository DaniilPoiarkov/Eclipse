using Eclipse.Application.Contracts.Users;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Core.Context;
using Eclipse.Core.Provider;
using Eclipse.Localization.Culture;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.Localization;

using Telegram.Bot;

namespace Eclipse.Pipelines.Jobs;

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

    public MoodRecordCollector(
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        IStringLocalizer<MoodRecordCollector> localizer,
        IMessageStore messageStore,
        IUserService userService,
        ICurrentCulture currentCulture)
    {
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _localizer = localizer;
        _messageStore = messageStore;
        _userService = userService;
        _currentCulture = currentCulture;
    }

    public async Task CollectAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return;
        }

        var user = result.Value;

        var key = new PipelineKey(user.ChatId);
        await _pipelineStore.RemoveAsync(key, cancellationToken);

        var pipeline = (_pipelineProvider.Get("/href_mood_records_add") as EclipsePipelineBase)!;

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

        await _pipelineStore.SetAsync(key, pipeline, cancellationToken);

        if (message is not null)
        {
            await _messageStore.SetAsync(new MessageKey(message.Chat.Id), message, cancellationToken);
        }
    }
}

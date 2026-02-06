using Eclipse.Core.Context;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.Stores;
using Eclipse.Core.UpdateParsing;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.Stores.Users;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using System.Reflection;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Pipelines.UpdateHandler;

internal sealed class EclipseUpdateHandler : IEclipseUpdateHandler
{
    public HandlerType Type => HandlerType.Active;

    private readonly ILogger<EclipseUpdateHandler> _logger;

    private readonly IUserStore _userStore;

    private readonly IPipelineStore _pipelineStore;

    private readonly IMessageStore _messageStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly IUpdateParser _updateParser;

    private readonly IStringLocalizer<EclipseUpdateHandler> _localizer;

    private static readonly UpdateType[] _allowedUpdateTypes =
    [
        UpdateType.Message,
        UpdateType.CallbackQuery,
        UpdateType.MyChatMember
    ];

    public EclipseUpdateHandler(
        ILogger<EclipseUpdateHandler> logger,
        IPipelineStore pipelineStore,
        IUserStore userStore,
        IUpdateParser updateParser,
        IPipelineProvider pipelineProvider,
        IMessageStore messageStore,
        IStringLocalizer<EclipseUpdateHandler> localizer)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _userStore = userStore;
        _pipelineProvider = pipelineProvider;
        _updateParser = updateParser;
        _messageStore = messageStore;
        _localizer = localizer;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!_allowedUpdateTypes.Contains(update.Type))
        {
            _logger.LogWarning("Update of type {updateType} is not supported", update.Type);
            return;
        }

        var context = _updateParser.Parse(update);

        if (context is null)
        {
            _logger.LogWarning("Context is null after parsing update of type {updateType}", update.Type);
            return;
        }

        var result = await HandleAndGetResultAsync(botClient, update, context, cancellationToken);

        if (result is RedirectResult redirect)
        {
            var command = redirect.PipelineType.GetCustomAttribute<RouteAttribute>()?.Command ?? string.Empty;

            var redirectUpdate = new Update
            {
                Message = new Message
                {
                    Text = command,
                    From = update.ExtractSender(),
                }
            };

            await HandleAndGetResultAsync(botClient, redirectUpdate, context, cancellationToken);
        }

        await _userStore.CreateOrUpdateAsync(context.User, update, cancellationToken);
    }

    private async Task<IResult> HandleAndGetResultAsync(ITelegramBotClient botClient, Update update, MessageContext context, CancellationToken cancellationToken)
    {
        // TODO: Refactor
        // =================================
        EclipsePipelineBase? pipeline = null;

        if (update.TryExtractMessage(out var trigger))
        {
            pipeline = await _pipelineStore.Get(context.ChatId, trigger.Id, cancellationToken) as EclipsePipelineBase;
        }

        pipeline ??= await _pipelineStore.Get(context.ChatId, cancellationToken) as EclipsePipelineBase
            ?? _pipelineProvider.Get(update) as EclipsePipelineBase
            ?? new EclipseNotFoundPipeline();
        // =================================

        await _pipelineStore.Remove(context.ChatId, pipeline, cancellationToken);

        pipeline.SetLocalizer(_localizer);
        pipeline.SetUpdate(update);

        var result = await pipeline.RunNext(context, cancellationToken);
        var message = await result.SendAsync(botClient, cancellationToken);

        if (message is not null)
        {
            await _messageStore.Set(context.ChatId, message, cancellationToken);
        }

        if (pipeline.IsFinished)
        {
            return result;
        }

        if (pipeline.GetType().GetCustomAttribute<MappedToMessageAttribute>() is not null && trigger is not null)
        {
            await _pipelineStore.Set(context.ChatId, trigger.Id, pipeline, cancellationToken);
            return result;
        }

        await _pipelineStore.Set(context.ChatId, pipeline, cancellationToken);
        return result;
    }
}

using Eclipse.Core.Builder;
using Eclipse.Core.Context;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.Stores;
using Eclipse.Core.UpdateParsing;
using Eclipse.Core.Updates;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Handlers;

internal sealed class EclipseUpdateHandler : IEclipseUpdateHandler
{
    public HandlerType Type => HandlerType.Active;

    private readonly ILogger<EclipseUpdateHandler> _logger;

    private readonly IPipelineStore _pipelineStore;

    private readonly IMessageStore _messageStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly IUpdateParser _updateParser;

    private readonly IUpdateAccessor _updateAccessor;

    private readonly IOptions<CoreOptions> _options;

    private readonly IEnumerable<IPipelinePreConfigurator> _configurators;

    public EclipseUpdateHandler(
        ILogger<EclipseUpdateHandler> logger,
        IPipelineStore pipelineStore,
        IUpdateParser updateParser,
        IUpdateAccessor updateAccessor,
        IPipelineProvider pipelineProvider,
        IMessageStore messageStore,
        IOptions<CoreOptions> options,
        IEnumerable<IPipelinePreConfigurator> configurators)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _updateParser = updateParser;
        _updateAccessor = updateAccessor;
        _messageStore = messageStore;
        _options = options;
        _configurators = configurators;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!_options.Value.AllowedUpdateTypes.Contains(update.Type))
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

        _updateAccessor.Set(update);

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
    }

    private async Task<IResult> HandleAndGetResultAsync(ITelegramBotClient botClient, Update update, MessageContext context, CancellationToken cancellationToken)
    {
        // TODO: Refactor
        // =================================
        // For potential input there can be only one active pipeline without message id even if previous message had an inline menu.
        // E.g. Receive reminder => reschedule. There is an inline menu but also ability to enter value manually.
        // For potential improvement we can update message text instead of sending new messages.
        IPipeline? pipeline = null;

        if (update is { CallbackQuery.Message: { } })
        {
            pipeline = await _pipelineStore.Get(context.ChatId, update.CallbackQuery.Message.Id, cancellationToken);
        }

        pipeline ??= await _pipelineStore.Get(context.ChatId, cancellationToken)
            ?? _pipelineProvider.Get(update);
        // =================================

        await _pipelineStore.Remove(context.ChatId, pipeline, cancellationToken);

        pipeline.SetUpdate(update);

        foreach (var configurator in _configurators)
        {
            configurator.Configure(update, pipeline);
        }

        var result = await pipeline.RunNext(context, cancellationToken);
        var message = await result.SendAsync(botClient, cancellationToken);

        if (message is not null)
        {
            await _messageStore.Set(context.ChatId, pipeline.GetType(), message, cancellationToken);
        }

        await _messageStore.RemoveOlderThan(context.ChatId,
            DateTime.UtcNow.AddDays(-_options.Value.MessagePersistanceInDays),
            cancellationToken
        );

        if (pipeline.IsFinished)
        {
            return result;
        }

        await ((message is { ReplyMarkup: InlineKeyboardMarkup _ })
            ? _pipelineStore.Set(context.ChatId, message.Id, pipeline, cancellationToken)
            : _pipelineStore.Set(context.ChatId, pipeline, cancellationToken));

        return result;
    }
}

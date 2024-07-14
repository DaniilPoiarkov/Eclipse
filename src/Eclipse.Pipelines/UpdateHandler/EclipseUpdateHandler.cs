using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Core.UpdateParsing;
using Eclipse.Localization;
using Eclipse.Localization.Exceptions;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;
using Eclipse.Pipelines.Users;

using Microsoft.Extensions.Logging;

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

    private readonly ICurrentTelegramUser _currentUser;

    private readonly IUpdateParser _updateParser;

    //private readonly IEclipseLocalizer _localizer;

    private readonly ILocalizer _localizer;


    private static readonly UpdateType[] _allowedUpdateTypes =
    [
        UpdateType.Message, UpdateType.CallbackQuery
    ];

    public EclipseUpdateHandler(
        ILogger<EclipseUpdateHandler> logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IUserStore userStore,
        ICurrentTelegramUser currentUser,
        IUpdateParser updateParser,
        IMessageStore messageStore,
        ILocalizer localizer)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _userStore = userStore;
        _currentUser = currentUser;
        _updateParser = updateParser;
        _messageStore = messageStore;
        _localizer = localizer;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!_allowedUpdateTypes.Contains(update.Type))
        {
            _logger.LogInformation("Update of type {updateType} is not supported", update.Type);
            return;
        }

        var context = _updateParser.Parse(update);

        if (context is null)
        {
            _logger.LogError("Context is null after parsing update of type {updateType}", update.Type);
            return;
        }

        _currentUser.SetCurrentUser(context.User);

        var key = new PipelineKey(context.ChatId);

        var pipeline = await GetPipelineAsync(context, key) as EclipsePipelineBase
            ?? new EclipseNotFoundPipeline();

        await _pipelineStore.RemoveAsync(key, cancellationToken);

        pipeline.SetLocalizer(_localizer);

        var result = await pipeline.RunNext(context, cancellationToken);

        var message = await result.SendAsync(botClient, cancellationToken);

        if (message is not null)
        {
            await _messageStore.SetAsync(new MessageKey(context.ChatId), message, cancellationToken);
        }

        if (!pipeline.IsFinished)
        {
            await _pipelineStore.SetAsync(key, pipeline, cancellationToken);
        }

        await _userStore.AddOrUpdateAsync(context.User, cancellationToken);
    }

    private async Task<PipelineBase> GetPipelineAsync(MessageContext context, PipelineKey key)
    {
        var pipeline = await _pipelineStore.GetOrDefaultAsync(key);

        if (pipeline is not null)
        {
            return pipeline;
        }

        if (context.Value.StartsWith('/'))
        {
            return _pipelineProvider.Get(context.Value);
        }

        try
        {
            return _pipelineProvider.Get(
                _localizer.ToLocalizableString(context.Value)
            );
        }
        catch (LocalizationNotFoundException)
        {
            // Retrieve INotFoundPipeline
            return _pipelineProvider.Get(string.Empty);
        }
    }
}

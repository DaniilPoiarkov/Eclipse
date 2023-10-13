using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

using Serilog;

using Eclipse.Core.Core;
using Eclipse.Infrastructure.Builder;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Core.UpdateParsing;
using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Pipelines;
using Eclipse.Localization.Exceptions;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;

namespace Eclipse.Pipelines.UpdateHandler;

internal class EclipseUpdateHandler : IEclipseUpdateHandler
{
    private readonly ILogger _logger;

    private readonly IIdentityUserStore _userStore;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ICurrentTelegramUser _currentUser;

    private readonly IUpdateParser _updateParser;

    private readonly IMessageStore _messageStore;

    private readonly IEclipseLocalizer _localizer;

    private readonly InfrastructureOptions _options;


    private static readonly UpdateType[] _allowedUpdateTypes = new[]
    {
        UpdateType.Message, UpdateType.CallbackQuery
    };

    public EclipseUpdateHandler(
        ILogger logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IIdentityUserStore userStore,
        ICurrentTelegramUser currentUser,
        IUpdateParser updateParser,
        IMessageStore messageStore,
        IEclipseLocalizer localizer,
        InfrastructureOptions options)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _userStore = userStore;
        _currentUser = currentUser;
        _updateParser = updateParser;
        _messageStore = messageStore;
        _localizer = localizer;
        _options = options;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error("Telegram error: {ex}", exception.Message);
        var options = _options.TelegramOptions;

        await botClient.SendTextMessageAsync(options.Chat, exception.Message, cancellationToken: cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (!_allowedUpdateTypes.Contains(update.Type))
        {
            _logger.Information("Update of type {updateType} is not supported", update.Type);
            return;
        }
        
        var context = _updateParser.Parse(update);
        
        if (context is null)
        {
            _logger.Error("Context is null after parsing update of type {updateType}", update.Type);
            return;
        }

        _currentUser.SetCurrentUser(context.User);

        var key = new PipelineKey(context.ChatId);

        var pipeline = GetPipeline(context, key) as EclipsePipelineBase
            ?? new EclipseNotFoundPipeline();
        
        _pipelineStore.Remove(key);

        _localizer.CheckCulture(context.ChatId);
        pipeline.SetLocalizer(_localizer);

        var result = await pipeline.RunNext(context, cancellationToken);

        var message = await result.SendAsync(botClient, cancellationToken);

        if (message is not null)
        {
            _messageStore.Set(new MessageKey(context.ChatId), message);
        }

        if (!pipeline.IsFinished)
        {
            _pipelineStore.Set(pipeline, key);
        }

        await _userStore.AddOrUpdate(context.User, cancellationToken);
    }

    private PipelineBase GetPipeline(MessageContext context, PipelineKey key)
    {
        var pipeline = _pipelineStore.GetOrDefault(key);

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
                _localizer.ToLocalizableString(context.Value));
        }
        catch (LocalizationNotFoundException)
        {
            // Retrieve INotFoundPipeline
            return _pipelineProvider.Get(string.Empty);
        }
    }
}

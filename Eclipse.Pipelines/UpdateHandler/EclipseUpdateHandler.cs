using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

using Serilog;

using Eclipse.Core.Core;
using Eclipse.Core.UpdateParsing;
using Eclipse.Common.Telegram;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Pipelines;
using Eclipse.Localization.Exceptions;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.Users;
using Eclipse.Pipelines.Stores.Messages;
using Eclipse.Pipelines.Stores.Pipelines;

using Microsoft.Extensions.Options;

namespace Eclipse.Pipelines.UpdateHandler;

internal sealed class EclipseUpdateHandler : IEclipseUpdateHandler
{
    private readonly ILogger _logger;

    private readonly IUserStore _userStore;

    private readonly IPipelineStore _pipelineStore;

    private readonly IMessageStore _messageStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ICurrentTelegramUser _currentUser;

    private readonly IUpdateParser _updateParser;

    private readonly IEclipseLocalizer _localizer;

    private readonly IOptions<TelegramOptions> _options;


    private static readonly UpdateType[] _allowedUpdateTypes =
    [
        UpdateType.Message, UpdateType.CallbackQuery
    ];

    public EclipseUpdateHandler(
        ILogger logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        IUserStore userStore,
        ICurrentTelegramUser currentUser,
        IUpdateParser updateParser,
        IMessageStore messageStore,
        IEclipseLocalizer localizer,
        IOptions<TelegramOptions> options)
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

        await botClient.SendTextMessageAsync(_options.Value.Chat, exception.Message, cancellationToken: cancellationToken);
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
            _pipelineStore.Set(key, pipeline);
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

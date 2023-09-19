using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

using Serilog;

using Eclipse.Core.Core;
using Eclipse.Infrastructure.Builder;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Core.UpdateParsing;

namespace Eclipse.Pipelines.UpdateHandler;

internal class EclipseUpdateHandler : IEclipseUpdateHandler
{
    private readonly ILogger _logger;

    private readonly ITelegramUserRepository _userRepository;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ICurrentTelegramUser _currentUser;

    private readonly IUpdateParser _updateParser;

    private readonly InfrastructureOptions _options;


    private static readonly UpdateType[] _allowedUpdateTypes = new[]
    {
        UpdateType.Message, UpdateType.CallbackQuery
    };

    public EclipseUpdateHandler(
        ILogger logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        ITelegramUserRepository userRepository,
        ICurrentTelegramUser currentUser,
        IUpdateParser updateParser,
        InfrastructureOptions options)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _updateParser = updateParser;
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

        var pipeline = _pipelineStore.GetOrDefault(key)
            ?? _pipelineProvider.Get(context.Value);

        _pipelineStore.Remove(key);

        var result = await pipeline.RunNext(context, cancellationToken);

        await result.SendAsync(botClient, cancellationToken);

        if (!pipeline.IsFinished)
        {
            _pipelineStore.Set(pipeline, key);
        }

        _userRepository.EnshureAdded(context.User);
    }
}

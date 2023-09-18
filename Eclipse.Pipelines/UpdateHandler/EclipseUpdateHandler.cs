using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Serilog;
using Eclipse.Core.Models;
using Eclipse.Core.Core;
using Eclipse.Infrastructure.Builder;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Contracts.Telegram.Pipelines;

namespace Eclipse.Pipelines.UpdateHandler;

internal class EclipseUpdateHandler : IEclipseUpdateHandler
{
    private readonly ILogger _logger;

    private readonly ITelegramUserRepository _userRepository;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly ICurrentTelegramUser _currentUser;

    private readonly InfrastructureOptions _options;

    public EclipseUpdateHandler(
        ILogger logger,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        InfrastructureOptions options,
        ITelegramUserRepository userRepository,
        ICurrentTelegramUser currentUser)
    {
        _logger = logger;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
        _options = options;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error("Telegram error: {ex}", exception.Message);
        var options = _options.TelegramOptions;

        await botClient.SendTextMessageAsync(options.Chat, exception.Message, cancellationToken: cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
        {
            _logger.Information("Update is not type of message");
            return;
        }

        var chatId = update.Message!.Chat.Id;
        var value = update.Message!.Text ?? string.Empty;

        var key = new PipelineKey(chatId);
        var user = new TelegramUser(update);

        _currentUser.SetCurrentUser(user);

        var pipeline = _pipelineStore.GetOrDefault(key)
            ?? _pipelineProvider.Get(value);

        _pipelineStore.Remove(key);

        var context = new MessageContext(chatId, value, user);

        var result = await pipeline.RunNext(context, cancellationToken);

        await result.SendAsync(botClient, cancellationToken);

        if (!pipeline.IsFinished)
        {
            _pipelineStore.Set(pipeline, key);
        }

        _userRepository.EnshureAdded(user);
    }
}

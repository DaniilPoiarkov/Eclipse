using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Serilog;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Core.Models;
using Eclipse.Application.Contracts.Telegram.Stores;
using Eclipse.Core.Core;
using Microsoft.Extensions.Options;
using Eclipse.Infrastructure.Builder;

namespace Eclipse.Application.Telegram;

internal class TelegramUpdateHandler : ITelegramUpdateHandler
{
    private readonly ILogger _logger;

    private readonly IUserStore _userStore;

    private readonly IPipelineStore _pipelineStore;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly InfrastructureOptions _options;

    public TelegramUpdateHandler(
        ILogger logger,
        IUserStore userStore,
        IPipelineStore pipelineStore,
        IPipelineProvider pipelineProvider,
        InfrastructureOptions options)
    {
        _logger = logger;
        _userStore = userStore;
        _pipelineStore = pipelineStore;
        _pipelineProvider = pipelineProvider;
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
        if (update.Type != UpdateType.Message)
        {
            _logger.Information("Update is not type of message");
            return;
        }

        var chatId = update.Message!.Chat.Id;
        var value = update.Message!.Text ?? string.Empty;

        var key = new PipelineKey(chatId);

        var pipeline = _pipelineStore.GetOrDefault(key);

        if (pipeline is not null)
        {
            _pipelineStore.Remove(key);
        }

        pipeline ??= _pipelineProvider.Get(value);

        _pipelineStore.Set(pipeline, key);

        var context = new MessageContext(chatId, value, new TelegramUser(update));

        var result = await pipeline.RunNext(context, cancellationToken);

        await result.SendAsync(botClient, cancellationToken);

        if (pipeline.IsFinished)
        {
            _pipelineStore.Remove(key);
        }

        _userStore.AddUser(new TelegramUser(update));
    }
}

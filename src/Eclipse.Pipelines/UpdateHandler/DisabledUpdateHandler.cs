using Eclipse.Common.Telegram;
using Eclipse.Core.UpdateParsing;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.UpdateHandler;

public sealed class DisabledUpdateHandler : IEclipseUpdateHandler
{
    public HandlerType Type => HandlerType.Disabled;

    private readonly IUpdateParser _parser;

    private readonly ILogger<DisabledUpdateHandler> _logger;

    private readonly IOptions<TelegramOptions> _options;

    public DisabledUpdateHandler(IUpdateParser parser, ILogger<DisabledUpdateHandler> logger, IOptions<TelegramOptions> options)
    {
        _parser = parser;
        _logger = logger;
        _options = options;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("Telegram error: {ex}", exception.Message);

        return botClient.SendTextMessageAsync(_options.Value.Chat, exception.Message, cancellationToken: cancellationToken);
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var context = _parser.Parse(update);

        if (context == null)
        {
            _logger.LogError("Context is null after parsing update of type {updateType}", update.Type);
            return Task.CompletedTask;
        }

        return botClient.SendTextMessageAsync(context.ChatId, "Eclipse is under maintenance right now. We'll come back soon!", cancellationToken: cancellationToken);
    }
}

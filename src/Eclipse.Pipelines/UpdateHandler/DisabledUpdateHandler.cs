using Eclipse.Core.UpdateParsing;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.UpdateHandler;

public sealed class DisabledUpdateHandler : IEclipseUpdateHandler
{
    public HandlerType Type => HandlerType.Disabled;

    private readonly IUpdateParser _parser;

    private readonly ILogger<DisabledUpdateHandler> _logger;

    public DisabledUpdateHandler(IUpdateParser parser, ILogger<DisabledUpdateHandler> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var context = _parser.Parse(update);

        if (context is null)
        {
            _logger.LogError("Context is null after parsing update of type {updateType}", update.Type);
            return Task.CompletedTask;
        }

        return botClient.SendTextMessageAsync(context.ChatId, "Eclipse is under maintenance right now. We'll come back soon!", cancellationToken: cancellationToken);
    }
}

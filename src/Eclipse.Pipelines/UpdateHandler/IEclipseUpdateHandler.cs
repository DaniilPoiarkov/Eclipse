using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Pipelines.UpdateHandler;

public interface IEclipseUpdateHandler
{
    HandlerType Type { get; }

    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default);
}

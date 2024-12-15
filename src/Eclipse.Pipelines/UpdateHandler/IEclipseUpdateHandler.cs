using Telegram.Bot.Types;
using Telegram.Bot;

namespace Eclipse.Pipelines.UpdateHandler;

public interface IEclipseUpdateHandler
{
    HandlerType Type { get; }

    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default);
}

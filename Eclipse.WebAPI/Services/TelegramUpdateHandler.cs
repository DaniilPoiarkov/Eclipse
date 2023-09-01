using Eclipse.Application.Contracts.UserStores;
using Eclipse.Infrastructure.Telegram;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ILogger = Serilog.ILogger;

namespace Eclipse.WebAPI.Services;

public class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ILogger _logger;

    private readonly IUserStore _userStore;

    public TelegramUpdateHandler(ILogger logger, IUserStore userStore)
    {
        _logger = logger;
        _userStore = userStore;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.Error("Telegram error: {ex}", exception.Message);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message)
        {
            _logger.Information("Update is not type of message");
            return;
        }

        _logger.Information("Recieved message from {chatId} (chatId)", update.Message!.Chat.Id);

        await botClient.SendTextMessageAsync(
            update.Message!.Chat.Id,
            "Hello! I'm Eclipse. Right now I'm having a rest, so see you later",
            cancellationToken: cancellationToken);

        _userStore.AddUser(new TelegramUser(update));
    }
}

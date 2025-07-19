using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Results;

public sealed class MenuResult : ResultBase
{
    public string Message { get; }

    public ReplyMarkup Menu { get; }

    public MenuResult(string message, ReplyMarkup menu)
    {
        Message = message;
        Menu = menu;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.SendMessage(
            chatId: ChatId,
            text: Message,
            replyMarkup: Menu,
            cancellationToken: cancellationToken
        );
    }
}

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Results;

public class MenuResult : ResultBase
{
    private readonly string _message;

    private readonly IReplyMarkup _menu;
    
    public MenuResult(string message, IReplyMarkup menu)
    {
        _message = message;
        _menu = menu;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.SendTextMessageAsync(
            chatId: ChatId,
            text: _message,
            replyMarkup: _menu,
            cancellationToken: cancellationToken);
    }
}

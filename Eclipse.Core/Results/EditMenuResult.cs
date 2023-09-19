using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Results;

public class EditMenuResult : ResultBase
{
    private readonly int _messageId;

    private readonly InlineKeyboardMarkup _menu;

    public EditMenuResult(int messageId, InlineKeyboardMarkup menu)
    {
        _messageId = messageId;
        _menu = menu;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.EditMessageReplyMarkupAsync(ChatId, _messageId, _menu, cancellationToken);
    }
}

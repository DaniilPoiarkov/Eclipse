using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Core.Results;

public sealed class EditMenuResult : ResultBase
{
    public int MessageId { get; }

    public InlineKeyboardMarkup Menu { get; }

    public EditMenuResult(int messageId, InlineKeyboardMarkup menu)
    {
        MessageId = messageId;
        Menu = menu;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.EditMessageReplyMarkup(ChatId, MessageId, Menu, cancellationToken: cancellationToken);
    }
}

using Eclipse.Core.Core;
using Eclipse.Core.Models;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Strategies;

public class CallbackQueryParseStrategy : IParseStrategy
{
    public UpdateType Type => UpdateType.CallbackQuery;

    public MessageContext? Parse(Update update)
    {
        var callback = update.CallbackQuery!;
        var from = callback.From;

        var name = from.FirstName;
        var surname = from.LastName ?? string.Empty;

        var user = new TelegramUser(from.Id, name, surname, from.Username ?? string.Empty);

        var value = callback.Data ?? string.Empty;

        return new MessageContext(from.Id, value, user);
    }
}

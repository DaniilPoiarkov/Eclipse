using Eclipse.Core.Core;
using Eclipse.Core.Models;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Strategies;

internal sealed class CallbackQueryParseStrategy : IParseStrategy
{
    public UpdateType Type => UpdateType.CallbackQuery;

    private readonly IServiceProvider _serviceProvider;

    public CallbackQueryParseStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MessageContext? Parse(Update update)
    {
        var callback = update.CallbackQuery!;
        var from = callback.From;

        var name = from.FirstName;
        var surname = from.LastName ?? string.Empty;

        var user = new TelegramUser(from.Id, name, surname, from.Username ?? string.Empty);

        var value = callback.Data ?? string.Empty;

        return new MessageContext(from.Id, value, user, _serviceProvider);
    }
}

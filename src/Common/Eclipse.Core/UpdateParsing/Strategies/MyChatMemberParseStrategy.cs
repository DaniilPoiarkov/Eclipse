using Eclipse.Core.Context;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Strategies;

internal sealed class MyChatMemberParseStrategy : IParseStrategy
{
    public UpdateType Type => UpdateType.MyChatMember;

    private readonly IServiceProvider _serviceProvider;
    public MyChatMemberParseStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MessageContext? Parse(Update update)
    {
        var message = update.MyChatMember!;

        var from = message.From;

        var user = new TelegramUser(from.Id,
            from.FirstName,
            from.LastName ?? string.Empty,
            from.Username ?? string.Empty
        );

        return new MessageContext(user.Id, string.Empty, user, _serviceProvider);
    }
}

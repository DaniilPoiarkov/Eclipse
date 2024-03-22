using Eclipse.Core.Core;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Strategies;

internal sealed class UnknownParseStrategy : IParseStrategy
{
    public UpdateType Type => UpdateType.Unknown;

    public MessageContext? Parse(Update update) => null;
}

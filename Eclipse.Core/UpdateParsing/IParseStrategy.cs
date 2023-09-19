using Eclipse.Core.Core;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing;

public interface IParseStrategy
{
    UpdateType Type { get; }

    MessageContext? Parse(Update update);
}

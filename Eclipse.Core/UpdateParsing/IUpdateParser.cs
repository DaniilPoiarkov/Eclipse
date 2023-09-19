using Eclipse.Core.Core;

using Telegram.Bot.Types;

namespace Eclipse.Core.UpdateParsing;

public interface IUpdateParser
{
    MessageContext? Parse(Update update);
}

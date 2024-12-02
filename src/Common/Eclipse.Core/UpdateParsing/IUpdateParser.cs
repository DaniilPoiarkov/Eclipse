using Eclipse.Core.Core;

using Telegram.Bot.Types;

namespace Eclipse.Core.UpdateParsing;

/// <summary>
/// Parses <a cref="Update"></a> to message context. If update cannot be parsed returns null;
/// </summary>
public interface IUpdateParser
{
    MessageContext? Parse(Update update);
}

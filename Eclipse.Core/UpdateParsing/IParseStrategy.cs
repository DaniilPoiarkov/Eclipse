using Eclipse.Core.Core;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing;

/// <summary>
/// Contains logic to parse <a cref="Update"></a>
/// </summary>
public interface IParseStrategy
{
    /// <summary>
    /// Defines which <a cref="UpdateType"></a> it can parse
    /// </summary>
    UpdateType Type { get; }

    MessageContext? Parse(Update update);
}

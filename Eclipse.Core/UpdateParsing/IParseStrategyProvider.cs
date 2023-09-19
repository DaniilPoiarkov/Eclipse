using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing;

/// <summary>
/// Provides suitable <a cref="IParseStrategy"></a> for provided <a cref="UpdateType"></a>
/// </summary>
public interface IParseStrategyProvider
{
    IParseStrategy Get(UpdateType type);
}

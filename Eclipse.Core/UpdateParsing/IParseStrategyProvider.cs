using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing;

public interface IParseStrategyProvider
{
    IParseStrategy Get(UpdateType type);
}

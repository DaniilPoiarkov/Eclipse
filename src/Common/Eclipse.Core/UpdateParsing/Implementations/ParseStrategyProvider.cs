using Eclipse.Core.UpdateParsing.Strategies;

using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Implementations;

internal sealed class ParseStrategyProvider : IParseStrategyProvider
{
    private readonly Dictionary<UpdateType, IParseStrategy> _strategies;

    public ParseStrategyProvider(IEnumerable<IParseStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Type, s => s);
    }

    public IParseStrategy Get(UpdateType type)
    {
        if (_strategies.TryGetValue(type, out var strategy))
        {
            return strategy;
        }

        return new UnknownParseStrategy();
    }
}

using Eclipse.Core.Core;

using Telegram.Bot.Types;

namespace Eclipse.Core.UpdateParsing.Implementations;

internal sealed class UpdateParser : IUpdateParser
{
    private readonly IParseStrategyProvider _parseStrategyProvider;

    public UpdateParser(IParseStrategyProvider parseStrategyProvider)
    {
        _parseStrategyProvider = parseStrategyProvider;
    }

    public MessageContext? Parse(Update update) =>
        _parseStrategyProvider.Get(update.Type).Parse(update);
}

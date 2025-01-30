using Telegram.Bot.Types;

namespace Eclipse.Core.UpdateParsing.Implementations;

internal sealed class UpdateProvider : IUpdateProvider
{
    private Update? _update;

    public Update Get()
    {
        return _update ?? throw new InvalidOperationException("Update not provided.");
    }

    public void Set(Update update)
    {
        _update = update;
    }
}

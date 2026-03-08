using Telegram.Bot.Types;

namespace Eclipse.Core.Updates;

internal sealed class UpdateAccessor : IUpdateAccessor
{
    public Update? Update { get; private set; }

    public void Set(Update update)
    {
        Update = update;
    }
}

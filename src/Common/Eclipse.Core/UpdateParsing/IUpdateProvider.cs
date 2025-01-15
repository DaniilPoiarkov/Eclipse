using Telegram.Bot.Types;

namespace Eclipse.Core.UpdateParsing;

public interface IUpdateProvider
{
    Update Get();

    void Set(Update update);
}

using Eclipse.Core.Models;

namespace Eclipse.Core.Core;

public interface ICurrentTelegramUser
{
    TelegramUser? GetCurrentUser();

    void SetCurrentUser(TelegramUser user);
}

using Eclipse.Core.Models;

namespace Eclipse.Core.Core;

/// <summary>
/// Simply provides access to current request <a cref="TelegramUser"></a>
/// </summary>
public interface ICurrentTelegramUser
{
    TelegramUser? GetCurrentUser();

    void SetCurrentUser(TelegramUser user);
}

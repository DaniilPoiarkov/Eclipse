using Eclipse.Core.Context;

namespace Eclipse.Core.CurrentUser;

// TODO: Drop
/// <summary>
/// Simply provides access to current request <a cref="TelegramUser"></a>
/// </summary>
public interface ICurrentTelegramUser
{
    TelegramUser? GetCurrentUser();

    void SetCurrentUser(TelegramUser user);
}

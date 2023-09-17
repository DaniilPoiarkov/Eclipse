using Eclipse.Core.Core;
using Eclipse.Core.Models;

namespace Eclipse.Core.CurrentUser;

internal sealed class CurrentTelegramUser : ICurrentTelegramUser
{
    private TelegramUser? _user;

    public TelegramUser? GetCurrentUser() => _user;

    public void SetCurrentUser(TelegramUser user) => _user = user;
}

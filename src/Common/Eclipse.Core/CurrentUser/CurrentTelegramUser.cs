using Eclipse.Core.Context;

namespace Eclipse.Core.CurrentUser;

// TODO: Drop
internal sealed class CurrentTelegramUser : ICurrentTelegramUser
{
    private TelegramUser? _user;

    public TelegramUser? GetCurrentUser() => _user;

    public void SetCurrentUser(TelegramUser user) => _user = user;
}

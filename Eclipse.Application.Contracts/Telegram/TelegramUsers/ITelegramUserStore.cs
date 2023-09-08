using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Telegram.TelegramUsers;

public interface ITelegramUserStore
{
    void EnsureAdded(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

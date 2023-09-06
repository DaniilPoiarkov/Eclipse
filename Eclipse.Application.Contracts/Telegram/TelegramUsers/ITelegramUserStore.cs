using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Telegram.TelegramUsers;

public interface ITelegramUserStore
{
    void AddUser(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Telegram.Stores;

public interface IUserStore
{
    void AddUser(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Telegram.Stores;

public interface IUserInfoStore
{
    void AddUser(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

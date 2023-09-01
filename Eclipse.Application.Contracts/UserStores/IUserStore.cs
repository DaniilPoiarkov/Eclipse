using Eclipse.Infrastructure.Telegram;

namespace Eclipse.Application.Contracts.UserStores;

public interface IUserStore
{
    void AddUser(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

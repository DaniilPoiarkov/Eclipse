using Eclipse.WebAPI.Services.TelegramServices;

namespace Eclipse.WebAPI.Services.UserStores;

public interface IUserStore
{
    void AddUser(TelegramUser user);

    IReadOnlyList<TelegramUser> GetUsers();
}

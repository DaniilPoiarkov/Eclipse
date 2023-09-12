using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Telegram.TelegramUsers;

public interface ITelegramUserRepository
{
    IReadOnlyList<TelegramUser> GetAll();

    void EnshureAdded(TelegramUser user);
}

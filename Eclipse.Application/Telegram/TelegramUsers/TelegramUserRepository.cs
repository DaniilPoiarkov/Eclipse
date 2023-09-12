using Eclipse.Application.Contracts.Google.Sheets.Users;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Models;

namespace Eclipse.Application.Telegram.TelegramUsers;

internal class TelegramUserRepository : ITelegramUserRepository
{
    private readonly ITelegramUserStore _userStore;

    private readonly IUsersSheetsService _sheetsService;

    public TelegramUserRepository(ITelegramUserStore userStore, IUsersSheetsService sheetsService)
    {
        _userStore = userStore;
        _sheetsService = sheetsService;
    }

    public void EnshureAdded(TelegramUser user)
    {
        if (_userStore.GetUsers().Any(u => u.Id == user.Id))
        {
            return;
        }

        var stored = _sheetsService.GetAll();

        if (stored.Any(u => u.Id == user.Id))
        {
            _userStore.EnsureAdded(user);
            return;
        }

        _sheetsService.Add(user);
    }

    public IReadOnlyList<TelegramUser> GetAll()
    {
        return _sheetsService.GetAll();
    }
}

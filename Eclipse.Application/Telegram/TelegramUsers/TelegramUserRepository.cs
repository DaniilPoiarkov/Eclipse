using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Core.Models;

namespace Eclipse.Application.Telegram.TelegramUsers;

internal class TelegramUserRepository : ITelegramUserRepository
{
    private readonly ITelegramUserStore _userStore;

    private readonly IEclipseSheetsService _sheetsService;

    public TelegramUserRepository(ITelegramUserStore userStore, IEclipseSheetsService sheetsService)
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

        var stored = _sheetsService.GetUsers();

        if (stored.Any(u => u.Id == user.Id))
        {
            _userStore.EnsureAdded(user);
            return;
        }

        _sheetsService.AddUser(user);
    }

    public IReadOnlyList<TelegramUser> GetAll()
    {
        return _sheetsService.GetUsers();
    }
}

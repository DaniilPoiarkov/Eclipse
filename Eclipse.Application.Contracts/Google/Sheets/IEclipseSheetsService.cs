using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Core.Models;

namespace Eclipse.Application.Contracts.Google.Sheets;

public interface IEclipseSheetsService
{
    IReadOnlyList<SuggestionDto> GetSuggestions();

    void AddSuggestion(SuggestionDto suggestion);

    IReadOnlyList<TelegramUser> GetUsers();

    void AddUser(TelegramUser user);
}

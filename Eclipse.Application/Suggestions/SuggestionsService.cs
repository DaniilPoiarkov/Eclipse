using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;

namespace Eclipse.Application.Suggestions;

internal class SuggestionsService : ISuggestionsService
{
    private readonly ISuggestionsSheetsService _sheetsService;

    private readonly ITelegramUserRepository _userRepository;

    public SuggestionsService(ISuggestionsSheetsService sheetsService, ITelegramUserRepository userRepository)
    {
        _sheetsService = sheetsService;
        _userRepository = userRepository;
    }

    public IReadOnlyList<SuggestionAndUserDto> GetDetailedInfo()
    {
        var suggestions = _sheetsService.GetAll();
        var users = _userRepository.GetAll();

        return suggestions.Join(users, s => s.ChatId, u => u.Id, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user
        }).ToList();
    }
}

using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.IdentityUsers;

namespace Eclipse.Application.Suggestions;

internal class SuggestionsService : ISuggestionsService
{
    private readonly ISuggestionsSheetsService _sheetsService;

    private readonly IIdentityUserStore _userStore;

    public SuggestionsService(ISuggestionsSheetsService sheetsService, IIdentityUserStore userStore)
    {
        _sheetsService = sheetsService;
        _userStore = userStore;
    }

    public async Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default)
    {
        var suggestions = _sheetsService.GetAll();
        var users = await _userStore.GetAllAsync(cancellationToken);

        return suggestions.Join(users, s => s.TelegramUserId, u => u.ChatId, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user
        }).ToList();
    }
}

using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Common.Results;

namespace Eclipse.Application.Suggestions;

internal sealed class SuggestionsService : ISuggestionsService
{
    private readonly ISuggestionsSheetsService _sheetsService;

    private readonly IIdentityUserService _userService;

    public SuggestionsService(ISuggestionsSheetsService sheetsService, IIdentityUserService userService)
    {
        _sheetsService = sheetsService;
        _userService = userService;
    }

    public async Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        var suggestion = new SuggestionDto
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            TelegramUserId = request.TelegramUserId,
            Text = request.Text,
        };

        await _sheetsService.AddAsync(suggestion, cancellationToken);
        
        return Result.Success();
    }

    public async Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default)
    {
        var suggestions = _sheetsService.GetAll();
        var users = await _userService.GetAllAsync(cancellationToken);

        return suggestions.Join(users, s => s.TelegramUserId, u => u.ChatId, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user
        }).ToList();
    }
}

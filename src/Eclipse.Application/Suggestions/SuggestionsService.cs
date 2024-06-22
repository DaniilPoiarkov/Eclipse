using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Users;
using Eclipse.Common.Results;
using Eclipse.Domain.Suggestions;

namespace Eclipse.Application.Suggestions;

internal sealed class SuggestionsService : ISuggestionsService
{
    private readonly IEclipseSheetsService<Suggestion> _sheetsService;

    private readonly IUserService _userService;

    public SuggestionsService(IEclipseSheetsService<Suggestion> sheetsService, IUserService userService)
    {
        _sheetsService = sheetsService;
        _userService = userService;
    }

    public async Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        var suggestion = Suggestion.Create(Guid.NewGuid(), request.Text, request.TelegramUserId, DateTime.UtcNow);

        await _sheetsService.AddAsync(suggestion, cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default)
    {
        var suggestionsRequest = _sheetsService.GetAllAsync(cancellationToken);
        var usersRequest = _userService.GetAllAsync(cancellationToken);

        await Task.WhenAll(suggestionsRequest, usersRequest);

        var suggestions = suggestionsRequest.Result;
        var users = usersRequest.Result;

        return suggestions.Join(users, s => s.TelegramUserId, u => u.ChatId, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user
        }).ToList();
    }
}

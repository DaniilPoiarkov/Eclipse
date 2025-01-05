using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Users;
using Eclipse.Common.Clock;
using Eclipse.Common.Results;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.Users;

namespace Eclipse.Application.Suggestions;

internal sealed class SuggestionsService : ISuggestionsService
{
    private readonly IEclipseSheetsService<Suggestion> _sheetsService;

    private readonly IUserRepository _userRepository;

    private readonly ITimeProvider _timeProvider;

    public SuggestionsService(IEclipseSheetsService<Suggestion> sheetsService, IUserRepository userRepository, ITimeProvider timeProvider)
    {
        _sheetsService = sheetsService;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        var suggestion = Suggestion.Create(Guid.CreateVersion7(), request.Text, request.TelegramUserId, _timeProvider.Now);

        await _sheetsService.AddAsync(suggestion, cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default)
    {
        var suggestionsRequest = _sheetsService.GetAllAsync(cancellationToken);
        var usersRequest = _userRepository.GetAllAsync(cancellationToken);

        await Task.WhenAll(suggestionsRequest, usersRequest);

        var suggestions = suggestionsRequest.Result;
        var users = usersRequest.Result;

        return suggestions.Join(users, s => s.TelegramUserId, u => u.ChatId, (suggestion, user) => new SuggestionAndUserDto
        {
            Id = suggestion.Id,
            Text = suggestion.Text,
            CreatedAt = suggestion.CreatedAt,
            User = user.ToSlimDto()
        }).ToList();
    }
}

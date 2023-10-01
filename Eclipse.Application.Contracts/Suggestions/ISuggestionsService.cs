namespace Eclipse.Application.Contracts.Suggestions;

public interface ISuggestionsService
{
    Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default);
}

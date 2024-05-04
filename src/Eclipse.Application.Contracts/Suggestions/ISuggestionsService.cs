using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Suggestions;

public interface ISuggestionsService
{
    Task<IReadOnlyList<SuggestionAndUserDto>> GetWithUserInfo(CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default);
}

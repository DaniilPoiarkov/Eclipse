using Eclipse.Common.Results;

namespace Eclipse.Suggestions.Application.Contracts;

public interface ISuggestionService
{
    Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SuggestionModel>> GetAllAsync(CancellationToken cancellationToken = default);
}

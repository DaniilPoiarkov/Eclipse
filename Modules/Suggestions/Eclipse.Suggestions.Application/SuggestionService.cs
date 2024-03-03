using Eclipse.Common.Results;
using Eclipse.Suggestions.Application.Contracts;
using Eclipse.Suggestions.Domain;

namespace Eclipse.Suggestions.Application;

internal sealed class SuggestionService : ISuggestionService
{
    private readonly ISuggestionRepository _repository;

    public SuggestionService(ISuggestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> CreateAsync(CreateSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        var suggestion = Suggestion.Create(Guid.NewGuid(), request.Text, request.ChatId, DateTime.UtcNow);

        await _repository.AddAsync(suggestion, cancellationToken);

        return Result.Success();
    }

    public async Task<IReadOnlyCollection<SuggestionModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var suggestions = await _repository.GetAllAsync(cancellationToken);

        return suggestions
            .Select(s => s.ToDto())
            .ToList();
    }
}

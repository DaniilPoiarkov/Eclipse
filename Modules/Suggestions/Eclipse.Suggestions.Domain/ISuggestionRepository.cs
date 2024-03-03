namespace Eclipse.Suggestions.Domain;

public interface ISuggestionRepository
{
    Task AddAsync(Suggestion suggestion, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken cancellationToken = default);
}

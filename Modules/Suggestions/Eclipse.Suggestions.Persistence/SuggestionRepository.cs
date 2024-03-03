using Eclipse.Application.Sheets;
using Eclipse.Suggestions.Domain;

namespace Eclipse.Suggestions.Persistence;

internal sealed class SuggestionRepository : ISuggestionRepository
{
    private readonly IEclipseSheetsService<Suggestion> _sheets;

    public SuggestionRepository(IEclipseSheetsService<Suggestion> sheets)
    {
        _sheets = sheets;
    }

    public Task AddAsync(Suggestion suggestion, CancellationToken cancellationToken = default)
    {
        return _sheets.AddAsync(suggestion, cancellationToken);
    }

    public Task<IReadOnlyList<Suggestion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _sheets.GetAllAsync(cancellationToken);
    }
}

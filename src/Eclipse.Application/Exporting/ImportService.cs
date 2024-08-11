using Eclipse.Application.Contracts.Exporting;
using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Application.Exporting;

internal sealed class ImportService : IImportService
{
    private readonly Dictionary<ImportType, IImportStrategy> _strategies;

    public ImportService(IEnumerable<IImportStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Type);
    }

    public Task<ImportResult<ImportEntityBase>> AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        return _strategies[ImportType.Reminders].ImportAsync(stream, cancellationToken);
    }

    public Task<ImportResult<ImportEntityBase>> AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        return _strategies[ImportType.TodoItems].ImportAsync(stream, cancellationToken);
    }

    public Task<ImportResult<ImportEntityBase>> AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default)
    {
        return _strategies[ImportType.Users].ImportAsync(stream, cancellationToken);
    }
}

using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Application.Contracts.Exporting;

public interface IImportService
{
    Task<ImportResult<ImportEntityBase>> AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task<ImportResult<ImportEntityBase>> AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task<ImportResult<ImportEntityBase>> AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default);
}

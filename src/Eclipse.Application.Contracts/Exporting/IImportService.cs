using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Application.Contracts.Exporting;

public interface IImportService
{
    Task<ImportResult<ImportUserDto>> AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task<ImportResult<ImportTodoItemDto>> AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task<ImportResult<ImportReminderDto>> AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default);
}

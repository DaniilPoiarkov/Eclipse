namespace Eclipse.Application.Contracts.Exporting;

public interface IImportService
{
    Task AddUsersAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task AddTodoItemsAsync(MemoryStream stream, CancellationToken cancellationToken = default);

    Task AddRemindersAsync(MemoryStream stream, CancellationToken cancellationToken = default);
}

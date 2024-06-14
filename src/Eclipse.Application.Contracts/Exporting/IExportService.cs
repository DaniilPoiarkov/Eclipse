namespace Eclipse.Application.Contracts.Exporting;

public interface IExportService
{
    Task<MemoryStream> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<MemoryStream> GetTodoItemsAsync(CancellationToken cancellationToken = default);

    Task<MemoryStream> GetRemindersAsync(CancellationToken cancellationToken = default);
}

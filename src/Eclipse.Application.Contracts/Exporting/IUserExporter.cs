namespace Eclipse.Application.Contracts.Exporting;

public interface IUserExporter
{
    Task<MemoryStream> ExportAllAsync(CancellationToken cancellationToken = default);
}

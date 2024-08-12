using Eclipse.Application.Contracts.Exporting;

namespace Eclipse.Application.Exporting;

public interface IImportStrategy
{
    ImportType Type { get; }

    Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default);
}

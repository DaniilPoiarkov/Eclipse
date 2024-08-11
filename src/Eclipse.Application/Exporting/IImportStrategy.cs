using Eclipse.Application.Contracts.Exporting;
using Eclipse.Domain.Shared.Importing;

namespace Eclipse.Application.Exporting;

public interface IImportStrategy
{
    ImportType Type { get; }

    Task<ImportResult<ImportEntityBase>> ImportAsync(MemoryStream stream, CancellationToken cancellationToken = default);
}

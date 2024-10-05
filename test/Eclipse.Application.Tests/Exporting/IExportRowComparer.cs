using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Application.Tests.Exporting;

internal interface IExportRowComparer<TEntity, TRow>
    where TEntity : Entity
    where TRow : ExportedRow
{
    bool Compare(TEntity entity, TRow row);
}

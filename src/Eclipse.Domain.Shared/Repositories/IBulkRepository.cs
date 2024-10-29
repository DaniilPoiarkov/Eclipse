using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Shared.Repositories;

public interface IBulkRepository<TEntity>
    where TEntity : Entity
{
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}

using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Shared.Repositories;

public interface IBulkUpdateRepository<TEntity>
    where TEntity : Entity
{
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}

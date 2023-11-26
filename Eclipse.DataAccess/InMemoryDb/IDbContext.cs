using Eclipse.Domain.Shared.Entities;

namespace Eclipse.DataAccess.InMemoryDb;

public interface IDbContext
{
    IList<TEntity> Set<TEntity>()
        where TEntity : Entity;

    void Update<TEntity>(TEntity entity)
        where TEntity : Entity;
}

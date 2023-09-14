using Eclipse.Domain.Base;

namespace Eclipse.DataAccess.DbContext;

public interface IDbContext
{
    IList<TEntity> Set<TEntity>()
        where TEntity : Entity;

    void Update<TEntity>(TEntity entity)
        where TEntity : Entity;
}

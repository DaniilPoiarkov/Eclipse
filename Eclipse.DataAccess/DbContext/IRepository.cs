using Eclipse.Domain.Base;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.DbContext;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    void Add(TEntity entity);

    void Delete(Guid id);

    void Update(TEntity entity);

    IReadOnlyList<TEntity> GetByExpression(Expression<Func<TEntity, bool>> expression);

    IReadOnlyList<TEntity> GetAll();

    TEntity? GetById(Guid id);
}

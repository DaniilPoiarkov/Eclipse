using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.CosmosDb;

internal abstract class CosmosRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly IContainer<TEntity> Container;

    public CosmosRepository(IContainer<TEntity> container)
    {
        Container = container;
    }

    public virtual Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        Container.CreateAsync(entity, cancellationToken);

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        Container.DeleteAsync(id, cancellationToken);

    public virtual Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        Container.FindAsync(id, cancellationToken);

    public virtual Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Container.GetAllAsync(cancellationToken);

    public virtual Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) =>
        Container.GetByExpressionAsync(expression, cancellationToken);

    public virtual Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        Container.UpdateAsync(entity, cancellationToken);
}

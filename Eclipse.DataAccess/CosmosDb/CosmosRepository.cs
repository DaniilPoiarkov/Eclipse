using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using MediatR;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.CosmosDb;

internal abstract class CosmosRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly IContainer<TEntity> Container;

    protected readonly IPublisher Publisher;

    public CosmosRepository(IContainer<TEntity> container, IPublisher publisher)
    {
        Container = container;
        Publisher = publisher;
    }

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await Container.CreateAsync(entity, cancellationToken);

        await TriggerDomainEvents(entity, cancellationToken);

        return result;
    }

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) =>
        Container.CountAsync(expression, cancellationToken);

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        Container.DeleteAsync(id, cancellationToken);

    public virtual Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        Container.FindAsync(id, cancellationToken);

    public virtual Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Container.GetAllAsync(cancellationToken);

    public virtual Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default) =>
        Container.GetByExpressionAsync(expression, cancellationToken);

    public Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default) =>
        Container.GetByExpressionAsync(expression, skipCount, takeCount, cancellationToken);

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await Container.UpdateAsync(entity, cancellationToken);
        
        await TriggerDomainEvents(entity, cancellationToken);

        return result;
    }

    protected virtual async Task TriggerDomainEvents(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity is not AggregateRoot aggregateRoot)
        {
            return;
        }

        var events = aggregateRoot.GetEvents();

        if (events.IsNullOrEmpty())
        {
            return;
        }

        foreach (var domainEvent in events)
        {
            await Publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

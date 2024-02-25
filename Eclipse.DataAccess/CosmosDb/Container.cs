using Eclipse.Domain.Shared.Entities;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.CosmosDb;

internal sealed class Container<TEntity> : IContainer<TEntity>
    where TEntity : Entity
{
    private readonly Container _container;

    public Container(Container container)
    {
        _container = container;
    }

    public IQueryable<TEntity> Items => _container.GetItemLinqQueryable<TEntity>(true).AsQueryable();

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var itemResponse = await _container.CreateItemAsync(entity, cancellationToken: cancellationToken);
        return itemResponse.Resource;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _container.DeleteItemAsync<TEntity>(id.ToString(), PartitionKey.None, cancellationToken: cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _container.UpsertItemAsync(entity, cancellationToken: cancellationToken);
        return response.Resource;
    }

    public FeedIterator<TEntity> GetFeedIterator(Expression<Func<TEntity, bool>> expression)
    {
        return _container.GetItemLinqQueryable<TEntity>()
            .Where(expression)
            .ToFeedIterator();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var iterator = _container.GetItemLinqQueryable<TEntity>()
            .ToFeedIterator();

        var results = new List<TEntity>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.Resource);
        }

        return results;
    }

    public async Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        using var iterator = GetFeedIterator(expression);

        var results = new List<TEntity>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.Resource);
        }

        return results;
    }

    public async Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var iterator = GetFeedIterator(entity => entity.Id == id);

        var response = await iterator.ReadNextAsync(cancellationToken);

        return response.Resource.SingleOrDefault();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await Items.Where(expression)
            .CountAsync(cancellationToken);
    }
}

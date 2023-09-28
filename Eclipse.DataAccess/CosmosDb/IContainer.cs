using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Repositories;

using Microsoft.Azure.Cosmos;

using System.Linq.Expressions;

namespace Eclipse.DataAccess.CosmosDb;

public interface IContainer<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    IQueryable<TEntity> Items { get; }

    FeedIterator<TEntity> GetFeedIterator(Expression<Func<TEntity, bool>> expression);
}

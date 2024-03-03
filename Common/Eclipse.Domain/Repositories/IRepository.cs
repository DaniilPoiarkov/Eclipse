using Eclipse.Domain.Entities;

namespace Eclipse.Domain.Repositories;

/// <summary>
/// Provides Api for retrieving data and perform CRUD operations
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> : IReadRepository<TEntity>, ICreateUpdateDeleteRepository<TEntity>
    where TEntity : Entity
{
    
}

using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.Shared.Repositories;

/// <summary>
/// Provides Api for retrieving data and perform CRUD operations
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> : IReadRepository<TEntity>, ICreateUpdateDeleteRepository<TEntity>
    where TEntity : Entity
{
    
}

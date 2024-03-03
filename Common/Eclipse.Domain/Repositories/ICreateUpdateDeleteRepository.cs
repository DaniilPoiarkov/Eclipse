using Eclipse.Domain.Entities;

namespace Eclipse.Domain.Repositories;

/// <summary>
/// Provides api for data manipulation
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface ICreateUpdateDeleteRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Creates <a cref="TEntity"></a> and returns it if entity was created successfully, otherwise returnes null
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates <a cref="TEntity"></a> and returns it if entity exist and was updated, otherwise returnes null
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entity with specified id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

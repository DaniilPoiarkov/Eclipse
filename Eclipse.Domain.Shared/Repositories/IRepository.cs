using Eclipse.Domain.Shared.Entities;
using System.Linq.Expressions;

namespace Eclipse.Domain.Shared.Repositories;

/// <summary>
/// Provides Api for retrieving data and perform CRUD operations
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Creates <a cref="TEntity"></a> and returns it if entity was created successfully, otherwise returnes null
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates <a cref="TEntity"></a> and returns it if entity exist and was updated, otherwise returnes null
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entity with specified id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returnes all <a cref="TEntity"></a> records
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returnes <a cref="TEntity"></a> records which match the expression
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <a cref="TEntity"></a> with specified id, if entity was not found returns null
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default);
}

using Eclipse.Domain.Entities;

using System.Linq.Expressions;

namespace Eclipse.Domain.Repositories;

/// <summary>
/// Provides api for data retrieving
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IReadRepository<TEntity>
    where TEntity : Entity
{
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
    /// Returnes <a cref="TEntity"></a> records which match the expression and spplies pagination.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="skipCount"></param>
    /// <param name="takeCount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, int skipCount, int takeCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns <a cref="TEntity"></a> with specified id, if entity was not found returns null
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns count of records, which match the expression
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
}

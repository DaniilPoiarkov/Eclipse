using Eclipse.Domain.Entities;

using System.Linq.Expressions;

namespace Eclipse.Domain.Repositories;

public static class RepositoryExtensions
{
    /// <summary>
    /// Determines weather at least one element match the expression
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="repository"></param>
    /// <param name="expression"></param>
    /// <param name="cancellationToken"></param>
    /// <returns><c>true</c> if at least one element match the expression, otherwise <c>false</c></returns>
    public static async Task<bool> ContainsAsync<TEntity>(this IReadRepository<TEntity> repository, Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        var count = await repository.CountAsync(expression, cancellationToken);
        return count > 0;
    }
}

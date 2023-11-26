using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.Exceptions;

namespace System.Linq;

public static class LinqExtensions
{

    /// <summary>Gets the entity by identifier.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entities">The entities.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    /// <exception cref="EntityNotFoundException">If entity with given id not exist is collection</exception>
    public static TEntity GetById<TEntity>(this IEnumerable<TEntity> entities, Guid id)
        where TEntity : Entity
    {
        return entities.FirstOrDefault(e => e.Id == id)
            ?? throw new EntityNotFoundException(typeof(TEntity));
    }
}

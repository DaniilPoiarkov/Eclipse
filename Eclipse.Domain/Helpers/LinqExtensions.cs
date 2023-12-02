using Eclipse.Domain.Exceptions;
using Eclipse.Domain.Shared.Entities;

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

    /// <summary>Gets the random item.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">The items.</param>
    /// <returns>Random item</returns>
    public static T GetRandomItem<T>(this IList<T> items)
    {
        return items[Random.Shared.Next(0, items.Count)];
    }

    /// <summary>
    /// Determines whether enumerable is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    ///   <c>true</c> if [is null or empty] [the specified source]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        if (source is null)
        {
            return true;
        }

        if (source is ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        if (source is IReadOnlyCollection<T> readOnlyCollection)
        {
            return readOnlyCollection.Count == 0;
        }

        if (source.TryGetNonEnumeratedCount(out var count))
        {
            return count == 0;
        }

        return source.Any();
    }
}

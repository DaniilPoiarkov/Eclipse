namespace System.Linq;

public static class LinqExtensions
{
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

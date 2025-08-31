namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginatedList<T>
{
    public ICollection<T> Items { get; private set; }

    public int Pages { get; private set; }

    public int TotalCount { get; private set; }

    public int Count { get; }

    /// <summary>
    /// Initializes the <a cref="PaginatedList{T}"></a>
    /// </summary>
    /// <param name="items">The items after applying pagination</param>
    /// <param name="pages">Total pages</param>
    /// <param name="totalCount">Total items count</param>
    public PaginatedList(ICollection<T> items, int pages, int totalCount)
    {
        Items = items;
        Pages = pages;
        TotalCount = totalCount;
        Count = items.Count;
    }

    /// <summary>
    /// Creates paginated list
    /// </summary>
    /// <param name="items">Already paged items</param>
    /// <param name="totalCount">Total items count</param>
    /// <param name="pageSize">Size of the page</param>
    /// <returns></returns>
    public static PaginatedList<T> Create(IEnumerable<T> items, int totalCount, int pageSize)
    {
        if (pageSize <= decimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        var pagesCount = (int)Math.Ceiling((double)totalCount / pageSize);
        return new PaginatedList<T>([.. items], pagesCount, totalCount);
    }
}

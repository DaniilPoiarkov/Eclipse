namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; private set; } = [];

    public int Pages { get; private set; }

    public int TotalCount { get; private set; }

    public int Count { get; }

    private static readonly int _minPage = 1;

    private static readonly int _minPageSize = 1;

    /// <summary>
    /// Initializes the <a cref="PaginatedList{T}"></a>
    /// </summary>
    /// <param name="items">The items after applying pagination</param>
    /// <param name="pages">Total pages</param>
    /// <param name="totalCount">Total items count</param>
    public PaginatedList(T[] items, int pages, int totalCount)
    {
        Items = items;
        Pages = pages;
        TotalCount = totalCount;
        Count = items.Length;
    }

    /// <summary>
    /// Creates <a cref="PaginatedList{T}"></a>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// page is less than 1
    /// or
    /// pageSize is less than 1
    /// </exception>
    public static PaginatedList<T> Create(IEnumerable<T> source, int page, int pageSize)
    {
        if (page < _minPage)
        {
            throw new ArgumentException($"Page indexing must start from {_minPage} exclusively.", nameof(page));
        }

        if (pageSize < _minPageSize)
        {
            throw new ArgumentException($"Page size must be at least {_minPageSize} element.", nameof(pageSize));
        }

        var itemsCount = source.TryGetNonEnumeratedCount(out var count)
            ? count
            : source.Count();

        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        var pages = GetPagesCount(itemsCount, pageSize);

        return new PaginatedList<T>(items, pages, itemsCount);
    }

    public static int GetPagesCount(int allIntesCount, int pageSize)
    {
        return (int)Math.Ceiling((double)allIntesCount / pageSize);
    }
}

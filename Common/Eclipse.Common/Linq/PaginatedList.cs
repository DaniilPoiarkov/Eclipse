namespace Eclipse.Common.Linq;

[Serializable]
public sealed class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; private set; } = [];

    public int Pages { get; private set; }

    public int TotalCount { get; private set; }

    public int Count { get; }

    public PaginatedList(T[] items, int pages, int totalCount)
    {
        Items = items;
        Pages = pages;
        TotalCount = totalCount;
        Count = items.Length;
    }

    public static PaginatedList<T> Create(IEnumerable<T> source, int page, int pageSize)
    {
        var itemsCount = source.TryGetNonEnumeratedCount(out var count)
            ? count
            : source.Count();

        var items = source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        var pages = (int)Math.Ceiling((double)itemsCount / pageSize);

        return new PaginatedList<T>(items, pages, itemsCount);
    }
}

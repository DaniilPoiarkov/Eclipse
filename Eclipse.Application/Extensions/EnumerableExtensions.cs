namespace Eclipse.Application.Extensions;

public static class EnumerableExtensions
{
    public static T GetRandomItem<T>(this IList<T> items)
    {
        return items[Random.Shared.Next(0, items.Count)];
    }
}

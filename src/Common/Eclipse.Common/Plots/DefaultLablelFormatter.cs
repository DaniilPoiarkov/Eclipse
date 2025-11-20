namespace Eclipse.Common.Plots;

internal sealed class DefaultLablelFormatter<T> : ILaberFormatter<T>
{
    public string Format(T value)
    {
        return value?.ToString() ?? string.Empty;
    }
}

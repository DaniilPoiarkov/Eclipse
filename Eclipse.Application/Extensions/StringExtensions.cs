using System.Diagnostics.CodeAnalysis;

namespace System;

public static class StringExtensions
{
    public static bool EqualsCurrentCultureIgnoreCase(this string source, string other)
    {
        return source.Equals(other, StringComparison.CurrentCultureIgnoreCase);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? source)
    {
        return string.IsNullOrEmpty(source);
    }

    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    public static string FormattedOrEmpty(this string? str, Func<string, string> format)
    {
        if (str.IsNullOrEmpty() || str.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        return format(str);
    }

    public static bool TryParseAsTimeOnly(this string? str, out TimeOnly parsed)
    {
        parsed = default;

        if (str.IsNullOrEmpty())
        {
            return false;
        }

        var values = str.Split(':').ToArray();

        if (values.Length != 2)
        {
            return false;
        }

        if (!TryParse(values[0], out var hours) || !TryParse(values[1], out var minutes))
        {
            return false;
        }

        parsed = new TimeOnly(hours, minutes);

        return true;

        static bool TryParse(string value, out int num) => int.TryParse(value, out num);
    }
}

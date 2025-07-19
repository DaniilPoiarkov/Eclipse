using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130

namespace System;

public static class StringExtensions
{
    /// <summary>
    /// <code>
    /// return source.Equals(other, StringComparison.CurrentCultureIgnoreCase);
    /// </code>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool EqualsCurrentCultureIgnoreCase(this string source, string other)
    {
        return source.Equals(other, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// <code>
    /// return string.IsNullOrEmpty(source);
    /// </code>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? source)
    {
        return string.IsNullOrEmpty(source);
    }

    /// <summary>
    /// <code>
    /// return string.IsNullOrWhiteSpace(source);
    /// </code>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    /// <summary>
    /// If string is null, empty or whitespaces, then returns empty string, otherwise applies formatting and returns result
    /// </summary>
    /// <param name="str"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string FormattedOrEmpty(this string? str, Func<string, string> format)
    {
        if (str.IsNullOrEmpty() || str.IsNullOrWhiteSpace())
        {
            return string.Empty;
        }

        return format(str);
    }

    /// <summary>
    /// Tries parse string value as <a cref="TimeOnly"></a>.
    /// <para>
    /// Returns <c>true</c> if parsed successfully (e.g. "12:30"), otherwise <c>false</c>
    /// </para>
    /// </summary>
    /// <param name="str"></param>
    /// <param name="parsed"></param>
    /// <returns></returns>
    public static bool TryParseAsTimeOnly(this string? str, out TimeOnly parsed)
    {
        parsed = default;

        if (str.IsNullOrEmpty())
        {
            return false;
        }

        var values = str
            .Split(':', StringSplitOptions.RemoveEmptyEntries)
            .ToArray();

        if (values.Length != 2)
        {
            return false;
        }

        if (!TryParse(values[0], out var hours) || !TryParse(values[1], out var minutes))
        {
            return false;
        }

        try
        {
            parsed = new TimeOnly(hours, minutes);
            return true;
        }
        catch
        {
            return false;
        }

        static bool TryParse(string value, out int num) => int.TryParse(value, out num);
    }

    /// <summary>
    /// Joins the strings using specified separator.
    /// </summary>
    /// <param name="strings">The strings.</param>
    /// <param name="separator">The separator.</param>
    /// <returns></returns>
    public static string Join(this IEnumerable<string> strings, string separator)
    {
        return string.Join(separator, strings);
    }

    public static string EnsureEndsWith(this string str, char expected)
    {
        if (str.EndsWith(expected))
        {
            return str;
        }

        return $"{str}{expected}";
    }
}

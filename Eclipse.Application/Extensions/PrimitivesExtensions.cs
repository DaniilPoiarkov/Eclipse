namespace Eclipse.Application.Extensions;

public static class PrimitivesExtensions
{
    #region object

    public static Guid ToGuid(this object obj)
    {
        if (Guid.TryParse(obj.ToString(), out var guid))
        {
            return guid;
        }

        return default;
    }

    public static long ToLong(this object obj)
    {
        if (long.TryParse(obj.ToString(),out var value))
        {
            return value;
        }

        return default;
    }

    public static DateTime ToDateTime(this object obj)
    {
        if (DateTime.TryParse(obj.ToString(), out var dateTime))
        {
            return dateTime;
        }
        
        return default;
    }

    public static bool ToBool(this object obj)
    {
        if (bool.TryParse(obj.ToString(), out var result))
        {
            return result;
        }

        return default;
    }

    #endregion

    #region string

    public static string FormattedOrEmpty(this string? str, Func<string, string> format)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            return string.Empty;
        }

        return format(str);
    }

    #endregion
}

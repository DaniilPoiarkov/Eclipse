﻿#pragma warning disable IDE0130

namespace System;

public static class ObjectExtensions
{
    public static Guid ToGuid(this object? obj)
    {
        if (Guid.TryParse(obj?.ToString(), out var guid))
        {
            return guid;
        }

        return default;
    }

    public static long ToLong(this object? obj)
    {
        if (long.TryParse(obj?.ToString(), out var value))
        {
            return value;
        }

        return default;
    }

    public static DateTime ToDateTime(this object? obj)
    {
        if (DateTime.TryParse(obj?.ToString(), out var dateTime))
        {
            return dateTime;
        }

        return default;
    }
}

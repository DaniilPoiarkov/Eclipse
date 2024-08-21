using System.Diagnostics.CodeAnalysis;

namespace System;

public static class GuidExtensions
{
    /// <summary>
    /// Determines whether <a cref="Guid"></a> value is equals to <a cref="Guid.Empty"></a>.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>
    ///   <c>true</c> if equals <a cref="Guid.Empty"></a>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty([NotNullWhen(false)] this Guid guid)
    {
        return guid == Guid.Empty;
    }
}

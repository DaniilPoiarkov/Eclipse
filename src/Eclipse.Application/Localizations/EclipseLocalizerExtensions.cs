﻿using Eclipse.Common.Results;
using Eclipse.Localization;

namespace Eclipse.Application.Localizations;

public static class EclipseLocalizerExtensions
{
    /// <summary>
    /// Localizes error data to string
    /// </summary>
    /// <param name="localizer"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static string LocalizeError(this ILocalizer localizer, Error error)
    {
        ArgumentNullException.ThrowIfNull(localizer, nameof(localizer));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        try
        {
            var description = localizer[error.Description];
            return string.Format(description, error.Args);
        }
        catch
        {
            return error.Description;
        }
    }
}

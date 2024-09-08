using Eclipse.Common.Results;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Localizations;

public static class EclipseLocalizerExtensions
{
    /// <summary>
    /// Localizes error data to string
    /// </summary>
    /// <param name="localizer"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static string LocalizeError(this IStringLocalizer localizer, Error error)
    {
        ArgumentNullException.ThrowIfNull(localizer, nameof(localizer));
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        try
        {
            var localizedString = localizer[error.Description, error.Args];

            if (localizedString.ResourceNotFound)
            {
                return error.Description;
            }

            return localizedString;
        }
        catch
        {
            return error.Description;
        }
    }

    /// <summary>
    /// Converts to localized error. Uses <a cref="Error.Description"></a> as a key and <a cref="Error.Args"></a> as args for localized string.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <param name="localizer">The localizer.</param>
    /// <returns></returns>
    public static Error ToLocalized(this Error error, IStringLocalizer localizer)
    {
        return new Error(error.Code, localizer[error.Description, error.Args], error.Type, []);
    }
}

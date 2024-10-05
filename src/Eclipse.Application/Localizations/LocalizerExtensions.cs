using Eclipse.Common.Results;

using Microsoft.Extensions.Localization;

namespace Eclipse.Application.Localizations;

public static class LocalizerExtensions
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
}

using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.Contracts.Localizations;

/// <summary>
/// Wrapper around <a cref="Localization.ILocalizer"></a> to track current user culture
/// </summary>
public interface IEclipseLocalizer
{
    /// <summary>
    /// Localize given <a cref="key"></a> using culture of current user. Enshure to call <a cref="ResetCultureForUserWithChatIdAsync"></a> method to have consistent localization
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string this[string key] { get; }

    /// <summary>
    /// Convert localized string to it's localizable key
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="LocalizationNotFoundException"></exception>
    string ToLocalizableString(string value);

    /// <summary>
    /// Updates culture for as for user with specified id
    /// </summary>
    /// <param name="id"></param>
    Task ResetCultureForUserWithChatIdAsync(long id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Converts Localizable exception to user-friendly message
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    string FormatLocalizedException(LocalizedException exception, string? culture = null);
}

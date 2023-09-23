using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.Contracts.Localizations;

/// <summary>
/// Wrapper around <a cref="Eclipse.Localization.Localizers.ILocalizer"></a> to track current user culture
/// </summary>
public interface IEclipseLocalizer
{
    string this[string key] { get; }

    string ToLocalizableString(string value);

    /// <summary>
    /// Updates culture for as for user with specified id
    /// </summary>
    /// <param name="id"></param>
    void CheckCulture(long id);

    string FormatLocalizedException(LocalizedException exception, string? culture = null);
}

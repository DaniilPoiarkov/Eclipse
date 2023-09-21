using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Localizers;

/// <summary>
/// Provides api to localize string
/// </summary>
public interface ILocalizer
{
    string this[string key, string? culture = null] { get; }

    string FormatLocalizedException(LocalizedException exception, string? culture = null);
}

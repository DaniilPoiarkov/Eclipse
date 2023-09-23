using Eclipse.Localization.Exceptions;

namespace Eclipse.Application.Contracts.Localizations;

public interface IEclipseLocalizer
{
    string this[string key] { get; }

    string ToLocalizableString(string value);

    void SetCulture(long id);

    string FormatLocalizedException(LocalizedException exception, string? culture = null);
}

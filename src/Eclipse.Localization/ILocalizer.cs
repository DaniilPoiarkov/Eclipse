﻿using Eclipse.Localization.Culture;
using Eclipse.Localization.Exceptions;

using Microsoft.Extensions.Localization;

namespace Eclipse.Localization;

/// <summary>
/// Provides api to localize string
/// </summary>
public interface ILocalizer
{
    /// <summary>
    /// Localize given <a cref="key"></a> using <a cref="culture"></a> specification
    /// </summary>
    /// <param name="key">String which need to be localized</param>
    /// <param name="culture">Cultute info</param>
    /// <returns></returns>
    LocalizedString this[string key, string? culture = null] { get; }

    /// <summary>
    /// Converts Localizable exception to user-friendly message
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    string FormatLocalizedException(LocalizedException exception, string? culture = null);

    /// <summary>
    /// Convert localized string to it's localizable key
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="LocalizationNotFoundException"></exception>
    string ToLocalizableString(string value);

    /// <summary>
    /// Uses the current culture.
    /// </summary>
    /// <param name="currentCulture">The current culture.</param>
    void UseCurrentCulture(ICurrentCulture currentCulture);
}

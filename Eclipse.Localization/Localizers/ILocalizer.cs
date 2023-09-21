﻿namespace Eclipse.Localization.Localizers;

/// <summary>
/// Provides api to localize string
/// </summary>
public interface ILocalizer
{
    string this[string key, string? culture = null] { get; }
}

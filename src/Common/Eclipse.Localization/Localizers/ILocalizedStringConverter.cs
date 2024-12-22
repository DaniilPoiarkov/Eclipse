namespace Eclipse.Localization.Localizers;

/// <summary>
/// 
/// </summary>
public interface ILocalizedStringConverter
{
    /// <summary>
    /// Converts localizaed string to it's key.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    string ToLocalizableString(string value);
}

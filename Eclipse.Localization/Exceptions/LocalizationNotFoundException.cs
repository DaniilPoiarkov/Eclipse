namespace Eclipse.Localization.Exceptions;

internal class LocalizationNotFoundException : ArgumentException
{
    public LocalizationNotFoundException(string value, string paramName)
        : base($"Localization info for value '{value}' not exists", paramName) { }
}

namespace Eclipse.Localization.Exceptions;

[Serializable]
public sealed class LocalizationNotFoundException : ArgumentException
{
    public LocalizationNotFoundException(string value, string paramName)
        : base($"Localization info for value '{value}' not found.", paramName) { }
}

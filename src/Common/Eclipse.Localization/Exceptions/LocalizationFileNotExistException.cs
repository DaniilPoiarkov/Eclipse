namespace Eclipse.Localization.Exceptions;

[Serializable]
public sealed class LocalizationFileNotExistException : ArgumentException
{
    public LocalizationFileNotExistException(string location)
        : base($"Localization file not found on path: {location}") { }

    public LocalizationFileNotExistException(string location, string culture)
        : base($"Localization file not found for culture '{culture}' on path: {location}") { }
}

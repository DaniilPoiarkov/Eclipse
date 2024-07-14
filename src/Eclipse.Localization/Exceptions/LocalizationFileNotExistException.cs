namespace Eclipse.Localization.Exceptions;

[Serializable]
internal sealed class LocalizationFileNotExistException : ArgumentException
{
    internal LocalizationFileNotExistException(string location)
        : base($"Localization file not found on path: {location}") { }

    internal LocalizationFileNotExistException(string location, string culture)
        : base($"Localization file not found for culture '{culture}' on path: {location}") { }
}

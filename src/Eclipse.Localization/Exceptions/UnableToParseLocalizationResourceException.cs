namespace Eclipse.Localization.Exceptions;

[Serializable]
internal sealed class UnableToParseLocalizationResourceException : ArgumentException
{
    internal UnableToParseLocalizationResourceException(string fileFullPath)
        : base($"Unable to parse localization file: {fileFullPath}") { }
}

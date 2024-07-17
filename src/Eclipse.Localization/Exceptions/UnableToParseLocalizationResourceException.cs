namespace Eclipse.Localization.Exceptions;

[Serializable]
public sealed class UnableToParseLocalizationResourceException : ArgumentException
{
    public UnableToParseLocalizationResourceException(string fileFullPath)
        : base($"Unable to parse localization file: {fileFullPath}") { }
}

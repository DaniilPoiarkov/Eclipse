namespace Eclipse.Localization.Exceptions;

[Serializable]
internal sealed class UnableToParseLocalizationResourceException : LocalizedException
{
    internal UnableToParseLocalizationResourceException(string file)
        : base("UnableToParseLocalizationExceptionMessage", file) { }
}

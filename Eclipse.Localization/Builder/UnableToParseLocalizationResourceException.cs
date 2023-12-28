using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Builder;

[Serializable]
internal sealed class UnableToParseLocalizationResourceException : LocalizedException
{
    internal UnableToParseLocalizationResourceException(string file)
        : base("UnableToParseLocalizationExceptionMessage", file) { }
}

using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Builder;

internal class UnableToParseLocalizationResourceException : LocalizedException
{
    public UnableToParseLocalizationResourceException(string file)
        : base("UnableToParseLocalizationExceptionMessage", file) { }
}

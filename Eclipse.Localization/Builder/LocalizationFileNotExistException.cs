using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Builder;

internal class LocalizationFileNotExistException : LocalizedException
{
    public LocalizationFileNotExistException(params string[] args)
        : base("LocalizationFileNotExistExceptionMessage", args) { }
}

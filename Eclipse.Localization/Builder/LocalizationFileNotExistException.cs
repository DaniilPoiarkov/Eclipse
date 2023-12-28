using Eclipse.Localization.Exceptions;

namespace Eclipse.Localization.Builder;

[Serializable]
internal sealed class LocalizationFileNotExistException : LocalizedException
{
    internal LocalizationFileNotExistException(params string[] args)
        : base("LocalizationFileNotExistExceptionMessage", args) { }
}

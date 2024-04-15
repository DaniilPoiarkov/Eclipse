namespace Eclipse.Localization.Exceptions;

[Serializable]
internal sealed class LocalizationFileNotExistException : LocalizedException
{
    internal LocalizationFileNotExistException(params string[] args)
        : base("LocalizationFileNotExistExceptionMessage", args) { }
}

namespace Eclipse.Localization.Exceptions;

public class LocalizedException : Exception
{
    public string[] Args { get; private set; }

    public LocalizedException(string message, params string[] args) : base(message)
    {
        Args = args ?? [];
    }
}

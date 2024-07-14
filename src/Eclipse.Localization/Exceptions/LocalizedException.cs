namespace Eclipse.Localization.Exceptions;

public class LocalizedException : Exception
{
    public object[] Args { get; private set; }

    public LocalizedException(string message, params object[] args) : base(message)
    {
        Args = args ?? [];
    }
}

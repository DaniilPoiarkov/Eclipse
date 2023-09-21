using Eclipse.Localization.Exceptions;

namespace Eclipse.Infrastructure.Internals.Telegram.Exceptions;

internal class TelegramException : LocalizedException
{
    public TelegramException(string message, params string[] args) : base(message, args)
    {
    }
}

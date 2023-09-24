namespace Eclipse.Infrastructure.Internals.Telegram.Exceptions;

internal class BotAlreadyRunningException : TelegramException
{
    public BotAlreadyRunningException()
        : base("Bot already running") { }
}

namespace Eclipse.Infrastructure.Exceptions;

internal class BotAlreadyRunningException : Exception
{
    public BotAlreadyRunningException()
        : base("Cannot start bot as it is already running") { }
}

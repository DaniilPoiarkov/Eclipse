namespace Eclipse.Infrastructure.Telegram;

public interface IEclipseStarter
{
    bool IsStarted { get; }

    Task StartAsync();
}

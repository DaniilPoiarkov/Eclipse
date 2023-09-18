namespace Eclipse.Infrastructure.Telegram;

/// <summary>
/// Simply starts bot to recieve updates
/// </summary>
public interface IEclipseStarter
{
    bool IsStarted { get; }

    Task StartAsync();
}

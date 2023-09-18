namespace Eclipse.Infrastructure.Telegram;

/// <summary>
/// Easy to use wrapper to interract with telegram API
/// </summary>
public interface ITelegramService
{
    Task Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

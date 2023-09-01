namespace Eclipse.Infrastructure.Telegram;

public interface ITelegramService
{
    Task Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

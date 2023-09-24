namespace Eclipse.Application.Contracts.Telegram;

public interface ITelegramService
{
    Task Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

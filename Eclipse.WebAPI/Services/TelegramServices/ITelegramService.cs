namespace Eclipse.WebAPI.Services.TelegramServices;

public interface ITelegramService
{
    Task Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

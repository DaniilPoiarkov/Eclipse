using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Telegram;

public interface ITelegramService
{
    Task<Result> Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

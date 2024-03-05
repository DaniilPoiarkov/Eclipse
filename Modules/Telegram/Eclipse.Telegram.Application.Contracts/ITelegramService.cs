using Eclipse.Common.Results;

namespace Eclipse.Telegram.Application.Contracts;

public interface ITelegramService
{
    Task<Result> Send(SendMessageModel message, CancellationToken cancellationToken = default);
}

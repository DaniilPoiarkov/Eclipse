using Telegram.Bot.Types;

namespace Eclipse.Core.Stores;

public interface IMessageStore
{    
    Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default);

    Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default);

    Task RemoveOlderThan(long chatId, DateTime date, CancellationToken cancellationToken = default);
}

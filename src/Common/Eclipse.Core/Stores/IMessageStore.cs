using Telegram.Bot.Types;

namespace Eclipse.Core.Stores;

public interface IMessageStore
{
    Task<Message?> Get(long chatId, int messageId, CancellationToken cancellationToken = default);

    Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default);

    Task<IEnumerable<Message>> GetAll(long chatId, CancellationToken cancellationToken = default);

    Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default);

    Task Remove(long chatId, int messageId, CancellationToken cancellationToken = default);

    Task RemoveAsync(long chatId, CancellationToken cancellationToken = default);
}

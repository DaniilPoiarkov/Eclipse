using Eclipse.Core.Stores;

using System.Collections.Concurrent;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.InMemory;

internal sealed class InMemoryMessageStore : IMessageStore
{
    private static readonly ConcurrentDictionary<long, List<MessageInfo>> _cache = [];

    public Task<Message?> Get(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult<Message?>(null);
        }

        return Task.FromResult(messages.FirstOrDefault(m => m.Message.Id == messageId)?.Message);
    }

    public Task<IEnumerable<Message>> GetAll(long chatId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult<IEnumerable<Message>>(messages.Select(x => x.Message));
        }

        return Task.FromResult<IEnumerable<Message>>([]);
    }

    public Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult(messages.LastOrDefault(m => m.PipelineType == pipelineType)?.Message);
        }

        return Task.FromResult<Message?>(null);
    }

    public Task Remove(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            messages.RemoveAll(m => m.Message.Id == messageId);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _cache.Remove(chatId, out _);
        return Task.CompletedTask;
    }

    public Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(chatId, out var messages))
        {
            messages = [];
            _cache[chatId] = messages;
        }

        messages.Add(new MessageInfo(pipelineType, message));
        return Task.CompletedTask;
    }

    private sealed record MessageInfo(Type PipelineType, Message Message);
}

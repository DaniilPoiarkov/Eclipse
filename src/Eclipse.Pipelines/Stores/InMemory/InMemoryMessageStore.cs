using Eclipse.Core.Stores;

using System.Collections.Concurrent;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.InMemory;

internal sealed class InMemoryMessageStore : IMessageStore
{
    private static readonly ConcurrentDictionary<long, List<Message>> _cache = [];

    public Task<Message?> Get(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult<Message?>(null);
        }

        return Task.FromResult(messages.FirstOrDefault(m => m.Id == messageId));
    }

    public Task<IEnumerable<Message>> GetAll(long chatId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult<IEnumerable<Message>>(messages);
        }

        return Task.FromResult<IEnumerable<Message>>([]);
    }

    public Task<Message?> GetLatestBotMessage(long chatId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult(messages.LastOrDefault());
        }

        return Task.FromResult<Message?>(null);
    }

    public Task Remove(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            messages.RemoveAll(m => m.Id == messageId);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(long chatId, CancellationToken cancellationToken = default)
    {
        _cache.Remove(chatId, out _);
        return Task.CompletedTask;
    }

    public Task Set(long chatId, Message message, CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(chatId, out var messages))
        {
            messages = [];
            _cache[chatId] = messages;
        }

        messages.Add(message);
        return Task.CompletedTask;
    }
}

using System.Collections.Concurrent;

using Telegram.Bot.Types;

namespace Eclipse.Core.Stores.InMemory;

internal sealed class InMemoryMessageStore : IMessageStore
{
    private static readonly ConcurrentDictionary<long, List<MessageInfo>> _cache = [];

    public Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            return Task.FromResult(messages.LastOrDefault(m => m.PipelineType == pipelineType)?.Message);
        }

        return Task.FromResult<Message?>(null);
    }

    public Task RemoveOlderThan(long chatId, DateTime date, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(chatId, out var messages))
        {
            messages.RemoveAll(m => m.Message.Date < date);
        }

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

using Eclipse.Common.Caching;
using Eclipse.Core.Stores;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores;

internal sealed class CacheMessageStore : IMessageStore
{
    private readonly ICacheService _cacheService;

    public CacheMessageStore(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public Task<Message?> GetLatestBotMessage(long chatId, Type pipelineType, CancellationToken cancellationToken = default)
    {
        return _cacheService.GetOrCreateAsync(
            $"stores:message:{chatId}:{pipelineType.FullName}",
            () => Task.FromResult<Message?>(default),
            GetCacheOptions(chatId),
            cancellationToken
        );
    }

    public async Task RemoveOlderThan(long chatId, DateTime date, CancellationToken cancellationToken = default)
    {
        var keys = await GetKeys(chatId, cancellationToken);

        foreach (var key in keys.Where(k => k.SetAt <= date))
        {
            await _cacheService.DeleteAsync(key.Key, cancellationToken);
        }

        await SetKeys(chatId, [.. keys.Where(k => k.SetAt > date)], cancellationToken);
    }

    public Task Set(long chatId, Type pipelineType, Message message, CancellationToken cancellationToken = default)
    {
        return _cacheService.SetAsync(
            $"stores:message:{chatId}:{pipelineType.FullName}",
            message,
            GetCacheOptions(chatId),
            cancellationToken
        );
    }

    private Task<List<MessageCacheKey>> GetKeys(long chatId, CancellationToken cancellationToken)
    {
        return _cacheService.GetOrCreateAsync(
            $"stores:message:{chatId}:keys",
            () => Task.FromResult<List<MessageCacheKey>>([]),
            GetCacheOptions(chatId),
            cancellationToken
        );
    }

    private Task SetKeys(long chatId, List<MessageCacheKey> keys, CancellationToken cancellationToken)
    {
        return _cacheService.SetAsync($"stores:message:{chatId}:keys", keys, GetCacheOptions(chatId), cancellationToken);
    }

    private CacheOptions GetCacheOptions(long chatId)
    {
        return new CacheOptions
        {
            Tags = [$"stores:message:{chatId}"],
            Expiration = CacheConsts.ThreeDays
        };
    }

    private record MessageCacheKey(string Key, DateTime SetAt);
}

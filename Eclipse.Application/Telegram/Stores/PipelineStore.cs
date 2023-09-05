using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Stores;
using Eclipse.Core.Pipelines;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Application.Telegram.Stores;

internal class PipelineStore : IPipelineStore
{
    private readonly ICacheService _cacheService;

    public PipelineStore(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public PipelineBase? GetOrDefault(PipelineKey key)
    {
        return _cacheService.Get<PipelineBase>(CreateKey(key));
    }

    public void Set(PipelineBase pipeline, PipelineKey key)
    {
        _cacheService.Set(CreateKey(key), pipeline);
    }

    public void Remove(PipelineKey key)
    {
        _cacheService.Delete(CreateKey(key));
    }

    private static CacheKey CreateKey(PipelineKey key) => new($"Pipeline-{key.ChatId}");
}

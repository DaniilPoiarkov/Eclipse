using Eclipse.Common.Caching;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Stores;

namespace Eclipse.Pipelines.Stores;

internal sealed class CachePipelineStore : IPipelineStore
{
    private readonly ICacheService _cacheService;

    private readonly IServiceProvider _serviceProvider;

    public CachePipelineStore(ICacheService cacheService, IServiceProvider serviceProvider)
    {
        _cacheService = cacheService;
        _serviceProvider = serviceProvider;
    }

    public async Task<IPipeline?> Get(long chatId, int messageId, CancellationToken cancellationToken = default)
    {
        var pipelineInfos = await GetAll(chatId, cancellationToken);

        var pipelineInfo = pipelineInfos.FirstOrDefault(k => k.MessageId == messageId);

        return await Resolve(pipelineInfo, cancellationToken);
    }

    public async Task<IPipeline?> Get(long chatId, CancellationToken cancellationToken = default)
    {
        var pipelineInfos = await GetAll(chatId, cancellationToken);

        var pipelineInfo = pipelineInfos.MaxBy(k => k.CreatedAt);

        return await Resolve(pipelineInfo, cancellationToken);
    }

    private async Task<IPipeline?> Resolve(PipelineInfo? pipelineInfo, CancellationToken cancellationToken = default)
    {
        if (pipelineInfo is null)
        {
            return null;
        }

        var type = Type.GetType(pipelineInfo.Type);

        if (type is null)
        {
            return null;
        }

        if (_serviceProvider.GetService(type) is not IPipeline pipeline)
        {
            return null;
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return pipeline;
    }

    public async Task Remove(long chatId, IPipeline pipeline, CancellationToken cancellationToken = default)
    {
        var pipelineInfos = await GetAll(chatId, cancellationToken);

        var removed = pipelineInfos.FirstOrDefault(k => k.Type == pipeline.GetType().AssemblyQualifiedName
            && k.StagesLeft == pipeline.StagesLeft
        );

        if (removed is not null)
        {
            pipelineInfos.Remove(removed);
        }

        await SetAll(chatId, pipelineInfos, cancellationToken);
    }

    public Task Set(long chatId, int messageId, IPipeline value, CancellationToken cancellationToken = default)
    {
        return SetInternal(chatId, messageId, value, cancellationToken);
    }

    public Task Set(long chatId, IPipeline value, CancellationToken cancellationToken = default)
    {
        return SetInternal(chatId, null, value, cancellationToken);
    }

    private async Task SetInternal(long chatId, int? messageId, IPipeline value, CancellationToken cancellationToken = default)
    {
        var pipelineInfo = new PipelineInfo(messageId, value.GetType().AssemblyQualifiedName ?? string.Empty, value.StagesLeft, DateTime.UtcNow);

        var keys = await GetAll(chatId, cancellationToken);

        keys.Add(pipelineInfo);

        await SetAll(chatId, keys, cancellationToken);
    }

    private Task<List<PipelineInfo>> GetAll(long chatId, CancellationToken cancellationToken)
    {
        return _cacheService.GetOrCreateAsync(
            $"stores:pipeline:{chatId}",
            () => Task.FromResult<List<PipelineInfo>>([]),
            GetCacheOptions(chatId),
            cancellationToken
        );
    }

    private Task SetAll(long chatId, List<PipelineInfo> keys, CancellationToken cancellationToken)
    {
        return _cacheService.SetAsync($"stores:pipeline:{chatId}", keys, GetCacheOptions(chatId), cancellationToken);
    }

    private CacheOptions GetCacheOptions(long chatId)
    {
        return new CacheOptions
        {
            Tags = [$"stores:pipeline:{chatId}"],
            Expiration = CacheConsts.ThreeDays,
        };
    }

    private record PipelineInfo(int? MessageId, string Type, int StagesLeft, DateTime CreatedAt);
}

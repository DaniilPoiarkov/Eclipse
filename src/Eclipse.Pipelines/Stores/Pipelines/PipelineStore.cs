using Eclipse.Application.Caching;
using Eclipse.Common.Cache;
using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.Pipelines;

internal sealed class PipelineStore : IPipelineStore
{
    private readonly ICacheService _cacheService;

    private readonly IServiceProvider _serviceProvider;

    public PipelineStore(ICacheService cacheService, IServiceProvider serviceProvider)
    {
        _cacheService = cacheService;
        _serviceProvider = serviceProvider;
    }

    public async Task<PipelineBase?> GetOrDefaultAsync(PipelineKey key, CancellationToken cancellationToken = default)
    {
        var pipelineInfo = await _cacheService.GetAsync<PipelineInfo>(key.ToCacheKey(), cancellationToken);

        if (pipelineInfo is null)
        {
            return null;
        }

        if (_serviceProvider.GetService(pipelineInfo.Type) is not PipelineBase pipeline)
        {
            return null;
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return pipeline;
    }

    public Task RemoveAsync(PipelineKey key, CancellationToken cancellationToken = default)
    {
        return _cacheService.DeleteAsync(key.ToCacheKey(), cancellationToken);
    }

    public Task SetAsync(PipelineKey key, PipelineBase value, CancellationToken cancellationToken = default)
    {
        return _cacheService.SetAsync(
            key.ToCacheKey(),
            new PipelineInfo(value.GetType(), value.StagesLeft),
            CacheConsts.OneDay,
            cancellationToken
        );
    }

    private record PipelineInfo(Type Type, int StagesLeft);
}

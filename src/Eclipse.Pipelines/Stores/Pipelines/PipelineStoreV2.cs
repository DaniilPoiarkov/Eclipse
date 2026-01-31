using Eclipse.Common.Caching;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider;

using Telegram.Bot.Types;

namespace Eclipse.Pipelines.Stores.Pipelines;

// Steps:
//      1. Retrieve pipeline info from cache
//      2. If not found, try to retrieve from pipeline provider
//      3. If found, skip stages to match the stages left in the info
//      4. Form key based on Update and pipeline type to allow multiple pipelines per user
internal sealed class PipelineStoreV2 : IPipelineStoreV2
{
    private readonly ICacheService _cacheService;

    private readonly IPipelineProvider _pipelineProvider;

    private readonly IServiceProvider _serviceProvider;

    public PipelineStoreV2(ICacheService cacheService, IPipelineProvider pipelineProvider, IServiceProvider serviceProvider)
    {
        _cacheService = cacheService;
        _pipelineProvider = pipelineProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<PipelineBase> Get(Update update, CancellationToken cancellationToken = default)
    {
        var pipelineInfo = await _cacheService.GetOrCreateAsync(
            GetPipelineKey(update),
            () => Task.FromResult<PipelineInfo?>(null),
            cancellationToken: cancellationToken
        );

        if (pipelineInfo is null)
        {
            return _pipelineProvider.Get(update);
        }

        if (_serviceProvider.GetService(pipelineInfo.Type) is not PipelineBase pipeline)
        {
            return _pipelineProvider.Get(update);
        }

        while (pipeline.StagesLeft != pipelineInfo.StagesLeft)
        {
            pipeline.SkipStage();
        }

        return pipeline;
    }

    public Task Remove(Update update, CancellationToken cancellationToken = default)
    {
        return _cacheService.DeleteAsync(GetPipelineKey(update), cancellationToken);
    }

    public Task Set(Update update, PipelineBase value, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.ThreeDays
        };

        return _cacheService.SetAsync(
            GetPipelineKey(update),
            new PipelineInfo(value.GetType(), value.StagesLeft),
            options,
            cancellationToken
        );
    }

    private string GetPipelineKey(Update update)
    {
        return update switch
        {
            { Message.From.Id: var userId } => $"pipeline:user:{userId}",
            { CallbackQuery.From.Id: var userId } => $"pipeline:user:{userId}",
            { InlineQuery.From.Id: var userId } => $"pipeline:user:{userId}",
            { ChosenInlineResult.From.Id: var userId } => $"pipeline:user:{userId}",
            _ => string.Empty,
        };
    }

    private record PipelineInfo(Type Type, int StagesLeft);
}

using Eclipse.Core.Pipelines;
using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Stores.Pipelines;

internal class PipelineStore : StoreBase<PipelineBase, PipelineKey>, IPipelineStore
{
    public PipelineStore(ICacheService cacheService) : base(cacheService)
    {
    }
}

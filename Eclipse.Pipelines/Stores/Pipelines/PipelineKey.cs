using Eclipse.Infrastructure.Cache;

namespace Eclipse.Pipelines.Stores.Pipelines;

public class PipelineKey : StoreKey
{
    public PipelineKey(long chatId) : base(chatId)
    {
    }

    public override CacheKey ToCacheKey()
    {
        return new CacheKey($"Pipeline-{ChatId}");
    }
}

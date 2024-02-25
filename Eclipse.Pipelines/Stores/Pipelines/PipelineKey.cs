using Eclipse.Common.Cache;

namespace Eclipse.Pipelines.Stores.Pipelines;

public sealed class PipelineKey : StoreKey
{
    public PipelineKey(long chatId) : base(chatId)
    {
    }

    public override CacheKey ToCacheKey()
    {
        return new CacheKey($"Pipeline-{ChatId}");
    }
}

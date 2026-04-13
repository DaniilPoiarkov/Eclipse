using Eclipse.Core;

namespace Eclipse.Pipelines.Stores;

public static class CacheStoresConfiguration
{
    public static CoreBuilder UseCacheStores(this CoreBuilder builder)
    {
        return builder.UsePipelineStore<CachePipelineStore>()
            .UseMessageStore<CacheMessageStore>();
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Pipelines.CachedServices;

internal static class CachedServiceProvider
{
    private static readonly string _key = "service-provider";

    private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public static IServiceProvider Services => _cache.Get<IServiceProvider>(_key)
        ?? new ServiceCollection().BuildServiceProvider();

    internal static void SetServiceProvider(IServiceProvider serviceProvider) =>
        _cache.Set(_key, serviceProvider);
}

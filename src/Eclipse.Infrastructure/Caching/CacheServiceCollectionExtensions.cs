using Eclipse.Common.Caching;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;

namespace Eclipse.Infrastructure.Caching;

public static class CacheServiceCollectionExtensions
{
    public static IServiceCollection UseDistributedCache(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();

        var options = new JsonSerializerSettings()
        {
            ContractResolver = PrivateMembersContractResolver.Instance
        };

        var serializer = new FusionCacheNewtonsoftJsonSerializer(options);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string is not provided");
        });

        services.AddFusionCache()
            .WithSerializer(serializer)
            .WithDistributedCache(sp => sp.GetRequiredService<IDistributedCache>())
            .AsHybridCache();

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }

    public static IServiceCollection UseDefaultCache(this IServiceCollection services)
    {
        var options = new JsonSerializerSettings()
        {
            ContractResolver = PrivateMembersContractResolver.Instance
        };

        var serializer = new FusionCacheNewtonsoftJsonSerializer(options);

        services.AddFusionCache()
            .WithSerializer(serializer)
            .AsHybridCache();

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}

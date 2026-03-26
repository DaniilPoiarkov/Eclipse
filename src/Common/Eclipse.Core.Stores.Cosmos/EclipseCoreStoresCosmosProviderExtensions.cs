using Eclipse.Core.Stores.Cosmos.Containers;
using Eclipse.Core.Stores.Cosmos.Stores;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Stores.Cosmos;

public static class EclipseCoreStoresCosmosProviderExtensions
{
    public static CoreBuilder UseCosmosStores(this CoreBuilder builder, Action<EclipseCoreStoresCosmosOptions> options)
    {
        builder.Services.Configure(options);

        builder.Services.AddSingleton<ICosmosClientResolver, CosmosClientResolver>()
            .AddScoped(sp => sp.GetRequiredService<ICosmosClientResolver>().Resolve());

        builder.Services.AddScoped<IContainerResolver, ContainerResolver>();

        builder.UsePipelineStore<CosmosPipelineStore>()
            .UseMessageStore<CosmosMessageStore>();

        builder.Services.AddHostedService<CosmosStoresInitializerHostedService>();

        return builder;
    }
}

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
            .AddScoped(sp => sp.GetRequiredService<ICosmosClientResolver>().Resolve())
            .AddScoped<IContainerResolver, ContainerResolver>()
            .AddScoped<ICosmosStore, CosmosStore>();

        builder.UsePipelineStore<CosmosPipelineStore>()
            .UseMessageStore<CosmosMessageStore>();

        builder.Services.AddHostedService<CosmosStoresInitializerHostedService>();

        return builder;
    }
}

public static class EclipseCoreStoresCosmosProviderBuilderExtensions
{
    public static CoreBuilder AddEnricher<TEnricher>(this CoreBuilder builder)
        where TEnricher : class, IDocumentEnricher
    {
        builder.Services.AddScoped<IDocumentEnricher, TEnricher>();

        return builder;
    }
}

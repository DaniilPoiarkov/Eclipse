using Azure.Identity;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Eclipse.Core.Stores.Cosmos;

internal sealed class CosmosClientResolver : ICosmosClientResolver
{
    private readonly IOptions<EclipseCoreStoresCosmosOptions> _options;

    public CosmosClientResolver(IOptions<EclipseCoreStoresCosmosOptions> options)
    {
        _options = options;
    }

    public CosmosClient Resolve()
    {
        return _options.Value.ConnectionType switch
        {
            CosmosConnectionType.ConnectionString => new CosmosClient(_options.Value.ConnectionString),
            CosmosConnectionType.ManagedIdentity => new CosmosClient(_options.Value.AccountEndpoint, new DefaultAzureCredential()),
            _ => throw new NotSupportedException($"Unsupported connection type: {_options.Value.ConnectionType}")
        };
    }
}

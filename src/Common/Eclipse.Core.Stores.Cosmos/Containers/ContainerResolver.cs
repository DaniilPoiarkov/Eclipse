using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Eclipse.Core.Stores.Cosmos.Containers;

internal sealed class ContainerResolver : IContainerResolver
{
    private readonly CosmosClient _client;

    private readonly IOptions<EclipseCoreStoresCosmosOptions> _options;

    public Container Container => _client.GetContainer(_options.Value.Database, _options.Value.Container);

    public ContainerResolver(CosmosClient client, IOptions<EclipseCoreStoresCosmosOptions> options)
    {
        _client = client;
        _options = options;
    }
}

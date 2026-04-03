using Microsoft.Azure.Cosmos;

namespace Eclipse.Core.Stores.Cosmos;

internal interface ICosmosClientResolver
{
    CosmosClient Resolve();
}

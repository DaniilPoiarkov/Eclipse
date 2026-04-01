using Microsoft.Azure.Cosmos;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal interface ICosmosStore
{
    Task<T?> Get<T>(QueryDefinition query, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAll<T>(QueryDefinition query, CancellationToken cancellationToken = default);

    Task Set(IStoreModel storeModel, CancellationToken cancellationToken = default);

    Task Remove(IStoreModel storeModel, CancellationToken cancellationToken = default);
}

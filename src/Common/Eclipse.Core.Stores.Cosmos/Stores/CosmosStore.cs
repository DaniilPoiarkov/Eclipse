using Eclipse.Core.Stores.Cosmos.Containers;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Net;
using System.Text.Json;

namespace Eclipse.Core.Stores.Cosmos.Stores;

internal sealed class CosmosStore : ICosmosStore
{
    private const int MaxItemsCount = 1;

    private readonly IContainerResolver _containerResolver;

    private readonly IEnumerable<IDocumentEnricher> _enrichers;

    private readonly IOptions<EclipseCoreStoresCosmosOptions> _options;

    private readonly ILogger<CosmosStore> _logger;

    private static readonly QueryRequestOptions DefaultQueryRequestOptions = new()
    {
        MaxItemCount = MaxItemsCount
    };

    public CosmosStore(IContainerResolver containerResolver, IEnumerable<IDocumentEnricher> enrichers, IOptions<EclipseCoreStoresCosmosOptions> options, ILogger<CosmosStore> logger)
    {
        _containerResolver = containerResolver;
        _enrichers = enrichers;
        _options = options;
        _logger = logger;
    }

    public async Task<T?> Get<T>(QueryDefinition query, CancellationToken cancellationToken = default)
    {
        using var iterator = _containerResolver.Container.GetItemQueryIterator<T>(query, requestOptions: DefaultQueryRequestOptions);

        var response = await iterator.ReadNextAsync(cancellationToken);

        return response.Resource.FirstOrDefault();
    }

    public async Task<IEnumerable<T>> GetAll<T>(QueryDefinition query, CancellationToken cancellationToken = default)
    {
        List<T> documents = [];

        using var iterator = _containerResolver.Container.GetItemQueryIterator<T>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);
            documents.AddRange(response.Resource);
        }

        return documents;
    }

    public async Task Set(IStoreModel storeModel, CancellationToken cancellationToken = default)
    {
        var document = storeModel.ToDictionary();

        foreach (var enricher in _enrichers)
        {
            enricher.Enrich(document);
        }

        var partitionKey = document.GetValueOrDefault(_options.Value.PartitionKey);

        _logger.LogInformation("Persisting {Discriminator} with id {Id} and partition key {PartitionKey}.", storeModel.Discriminator, document["id"], partitionKey);

        await _containerResolver.Container.CreateItemAsync(document,
            new PartitionKey(partitionKey?.ToString()),
            cancellationToken: cancellationToken
        );
    }

    public async Task Remove(IStoreModel storeModel, CancellationToken cancellationToken = default)
    {
        var query = new QueryDefinition($"select c.id as Id, c.{_options.Value.PartitionKey} as PartitionKey from c where c.Discriminator = @Discriminator and c.id = @id")
            .WithParameter("@id", storeModel.Id)
            .WithParameter("@Discriminator", storeModel.Discriminator);

        using var iterator = _containerResolver.Container.GetItemQueryIterator<DocumentWrapper>(
            query, requestOptions: DefaultQueryRequestOptions
        );

        var item = await iterator.ReadNextAsync(cancellationToken);

        if (item.StatusCode != HttpStatusCode.OK || item.Resource.Count() != MaxItemsCount)
        {
            _logger.LogWarning(
                "{Discriminator} with id {Id} not found. Skipping deletion. Query: {Query}. Status code: {StatusCode}, count: {Count}",
                storeModel.Discriminator, storeModel.Id, query.QueryText, item.StatusCode, item.Resource.Count()
            );
            return;
        }

        var document = item.Resource.First();

        _logger.LogInformation("Removing document with id {Id} and partition key {PartitionKey}.", document.Id, document.PartitionKey);

        await _containerResolver.Container.DeleteItemAsync<IStoreModel>(document.Id, new PartitionKey(document.PartitionKey), cancellationToken: cancellationToken);
    }

    private class DocumentWrapper
    {
        public required string Id { get; set; }
        public required string PartitionKey { get; set; }
    }
}

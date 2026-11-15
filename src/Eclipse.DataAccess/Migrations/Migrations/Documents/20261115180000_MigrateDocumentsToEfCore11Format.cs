using Eclipse.DataAccess.Cosmos;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Text.Json;
using System.Text.Json.Nodes;

namespace Eclipse.DataAccess.Migrations.Migrations.Documents;

/// <summary>
/// EF Core 11 stores the discriminator as <c>$type</c> instead of <c>Discriminator</c>,
/// and documents now use the raw <c>Id</c> value as Cosmos <c>id</c> instead of <c>{Discriminator}|{Id}</c>.
/// <para>
/// Cosmos <c>id</c> is immutable, so each document is re-created with the new <c>id</c> and the legacy one is deleted
/// in a single transactional batch (both share the same <c>/Id</c> partition key).
/// Documents are processed as raw JSON so all other properties are preserved as is.
/// </para>
/// </summary>
[Migration(20261115180000, "Move discriminator to $type and remove discriminator prefix from document id (EF Core 11)")]
internal sealed class _20261115180000_MigrateDocumentsToEfCore11Format : Migration
{
    private const int MaxDegreeOfParallelism = 10;

    private static readonly string[] _systemProperties = ["_rid", "_self", "_etag", "_attachments", "_ts"];

    private readonly CosmosClient _cosmosClient;

    private readonly IOptions<CosmosDbContextOptions> _options;

    private readonly ILogger<_20261115180000_MigrateDocumentsToEfCore11Format> _logger;

    public _20261115180000_MigrateDocumentsToEfCore11Format(
        CosmosClient cosmosClient,
        IOptions<CosmosDbContextOptions> options,
        ILogger<_20261115180000_MigrateDocumentsToEfCore11Format> logger)
    {
        _cosmosClient = cosmosClient;
        _options = options;
        _logger = logger;
    }

    public async override Task Migrate(CancellationToken cancellationToken = default)
    {
        var container = _cosmosClient.GetContainer(_options.Value.DatabaseId, _options.Value.Container);

        var documents = await GetLegacyDocumentsAsync(container, cancellationToken);

        _logger.LogInformation("Found {Count} documents in legacy format to migrate", documents.Count);

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = MaxDegreeOfParallelism,
            CancellationToken = cancellationToken,
        };

        await Parallel.ForEachAsync(documents, parallelOptions, (document, token) => MigrateDocumentAsync(container, document, token));

        _logger.LogInformation("Migrated {Count} documents to EF Core 11 format", documents.Count);
    }

    private static async Task<List<JsonObject>> GetLegacyDocumentsAsync(Container container, CancellationToken cancellationToken)
    {
        var query = new QueryDefinition(
            $"select * from c where c.{CosmosDocumentConsts.IdProperty} != c.{CosmosDocumentConsts.PartitionKeyProperty} " +
            $"or is_defined(c.{CosmosDocumentConsts.LegacyDiscriminatorProperty})"
        );

        using var iterator = container.GetItemQueryStreamIterator(query);

        var documents = new List<JsonObject>();

        while (iterator.HasMoreResults)
        {
            using var response = await iterator.ReadNextAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var page = await JsonNode.ParseAsync(response.Content, cancellationToken: cancellationToken)
                ?? throw new MigrationException("Failed to parse query response.");

            foreach (var document in page["Documents"]?.AsArray() ?? [])
            {
                if (document is null)
                {
                    continue;
                }

                documents.Add(document.DeepClone().AsObject());
            }
        }

        return documents;
    }

    private static async ValueTask MigrateDocumentAsync(Container container, JsonObject document, CancellationToken cancellationToken)
    {
        var legacyId = document[CosmosDocumentConsts.IdProperty]!.GetValue<string>();

        var id = document[CosmosDocumentConsts.PartitionKeyProperty]?.GetValue<string>()
            ?? throw new MigrationException($"Document '{legacyId}' has no '{CosmosDocumentConsts.PartitionKeyProperty}' property.");

        if (document.TryGetPropertyValue(CosmosDocumentConsts.LegacyDiscriminatorProperty, out var discriminator))
        {
            document.Remove(CosmosDocumentConsts.LegacyDiscriminatorProperty);

            if (!document.ContainsKey(CosmosDocumentConsts.DiscriminatorProperty))
            {
                document[CosmosDocumentConsts.DiscriminatorProperty] = discriminator;
            }
        }

        foreach (var property in _systemProperties)
        {
            document.Remove(property);
        }

        document[CosmosDocumentConsts.IdProperty] = id;

        using var content = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(document));

        var batch = container.CreateTransactionalBatch(new PartitionKey(id))
            .UpsertItemStream(content);

        if (!string.Equals(legacyId, id, StringComparison.Ordinal))
        {
            batch.DeleteItem(legacyId);
        }

        using var response = await batch.ExecuteAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new MigrationException($"Failed to migrate document '{legacyId}': {response.StatusCode} {response.ErrorMessage}");
        }
    }
}

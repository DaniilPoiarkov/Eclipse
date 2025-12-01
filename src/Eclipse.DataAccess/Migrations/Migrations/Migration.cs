using Microsoft.Azure.Cosmos;

namespace Eclipse.DataAccess.Migrations.Migrations;

internal abstract class Migration : IMigration
{
    public abstract Task Migrate(CancellationToken cancellationToken = default);

    protected static async Task<ItemResponse<object>[]> UpdateAsync(FeedIterator<DocumentId> iterator, PatchOperation operation, Container container, CancellationToken cancellationToken)
    {
        var updates = new List<Task<ItemResponse<object>>>();

        while (iterator.HasMoreResults)
        {
            var current = await iterator.ReadNextAsync(cancellationToken);

            foreach (var document in current.Resource)
            {
                updates.Add(
                    container.PatchItemAsync<object>(document.CosmosId, new PartitionKey(document.Id), [operation], cancellationToken: cancellationToken)
                );
            }
        }

        return await Task.WhenAll(updates);
    }
}

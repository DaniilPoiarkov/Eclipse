using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.TodoItems;

using Microsoft.Azure.Cosmos;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public class EclipseCosmosDbContext : CosmosDbContext
{
    public IContainer<TodoItem> TodoItems => Container<TodoItem>($"{nameof(TodoItem)}s");

    public EclipseCosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
        : base(client, options) { }

    internal override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var databaseResponse = await Client.CreateDatabaseIfNotExistsAsync(Options.DatabaseId, cancellationToken: cancellationToken);

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(TodoItem)}s", "/todoitems"),
            cancellationToken: cancellationToken);
    }
}

using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.IdentityUsers;

using Microsoft.Azure.Cosmos;
using Eclipse.Domain.Notifications;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public class EclipseCosmosDbContext : CosmosDbContext
{
    public IContainer<TodoItem> TodoItems => Container<TodoItem>($"{nameof(TodoItem)}s");

    public IContainer<IdentityUser> IdentityUsers => Container<IdentityUser>($"{nameof(IdentityUser)}s");

    public IContainer<Notification> Notifications => Container<Notification>($"{nameof(Notification)}s");

    public EclipseCosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
        : base(client, options) { }

    internal override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var databaseResponse = await Client.CreateDatabaseIfNotExistsAsync(Options.DatabaseId, cancellationToken: cancellationToken);

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(TodoItem)}s", "/todoitems"),
            cancellationToken: cancellationToken);

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(IdentityUser)}s", "/identityUsers"),
            cancellationToken: cancellationToken);

        await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(Notification)}s", "/notifications"),
            cancellationToken: cancellationToken);
    }
}

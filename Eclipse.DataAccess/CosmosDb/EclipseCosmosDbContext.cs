using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.TodoItems;
using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Reminders;

using Microsoft.Azure.Cosmos;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public class EclipseCosmosDbContext : CosmosDbContext
{
    public IContainer<TodoItem> TodoItems => Container<TodoItem>($"{nameof(TodoItem)}s");

    public IContainer<IdentityUser> IdentityUsers => Container<IdentityUser>($"{nameof(IdentityUser)}s");

    public IContainer<Reminder> Reminders => Container<Reminder>($"{nameof(Reminder)}s");

    public EclipseCosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
        : base(client, options) { }

    internal override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var response = await Client.CreateDatabaseIfNotExistsAsync(Options.DatabaseId, cancellationToken: cancellationToken);

        await response.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(TodoItem)}s", "/todoitems"),
            cancellationToken: cancellationToken);

        await response.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(IdentityUser)}s", "/identityUsers"),
            cancellationToken: cancellationToken);

        await response.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(Reminder)}s", "/reminders"),
            cancellationToken: cancellationToken);
    }
}

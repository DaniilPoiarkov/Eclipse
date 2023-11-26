using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.IdentityUsers;

using Microsoft.Azure.Cosmos;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public class EclipseCosmosDbContext : CosmosDbContext
{
    public IContainer<IdentityUser> IdentityUsers => Container<IdentityUser>($"{nameof(IdentityUser)}s");

    public EclipseCosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
        : base(client, options) { }

    internal override async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var response = await Client.CreateDatabaseIfNotExistsAsync(Options.DatabaseId, cancellationToken: cancellationToken);

        await response.Database.CreateContainerIfNotExistsAsync(
            new ContainerProperties($"{nameof(IdentityUser)}s", "/identityUsers"),
            cancellationToken: cancellationToken);
    }
}

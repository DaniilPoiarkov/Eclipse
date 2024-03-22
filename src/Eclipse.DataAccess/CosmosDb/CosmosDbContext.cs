using Eclipse.Domain.Shared.Entities;

using Microsoft.Azure.Cosmos;

namespace Eclipse.DataAccess.CosmosDb;

/// <summary>
/// Define your containers using <a cref="Container{TEntity}"></a> method
/// </summary>
public abstract class CosmosDbContext
{
    protected readonly CosmosClient Client;

    protected readonly CosmosDbContextOptions Options;

    public CosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
    {
        Client = client;
        Options = options;
    }

    /// <summary>
    /// Use it to initialize database and containers
    /// </summary>
    /// <returns></returns>
    public abstract Task InitializeAsync(CancellationToken cancellationToken = default);

    public IContainer<TEntity> Container<TEntity>(string name)
        where TEntity : Entity
    {
        return new Container<TEntity>(Client.GetContainer(Options.DatabaseId, name));
    }
}

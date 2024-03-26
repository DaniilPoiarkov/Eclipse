using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.IdentityUsers;

using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public sealed class EclipseCosmosDbContext : DbContext//CosmosDbContext
{
    //public IContainer<IdentityUser> IdentityUsers => Container<IdentityUser>($"{nameof(IdentityUser)}s");

    //public EclipseCosmosDbContext(CosmosClient client, CosmosDbContextOptions options)
    //    : base(client, options) { }

    public DbSet<IdentityUser> IdentityUsers { get; set; }

    public EclipseCosmosDbContext(DbContextOptions<EclipseCosmosDbContext> options) : base(options)
    {
        
    }

    //public override async Task InitializeAsync(CancellationToken cancellationToken = default)
    //{
    //    var response = await Client.CreateDatabaseIfNotExistsAsync(Options.DatabaseId, cancellationToken: cancellationToken);

    //    await response.Database.CreateContainerIfNotExistsAsync(
    //        new ContainerProperties($"{nameof(IdentityUser)}s", "/identityUsers"),
    //        cancellationToken: cancellationToken);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EclipseCosmosDbContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Cosmos;

internal sealed class EclipseDbContext : DbContext
{
    private readonly IOptions<CosmosDbContextOptions> _contextOptions;

    public EclipseDbContext(DbContextOptions<EclipseDbContext> options, IOptions<CosmosDbContextOptions> contextOptions)
        : base(options)
    {
        _contextOptions = contextOptions;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultContainer(_contextOptions.Value.Container);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EclipseDbContext).Assembly);
    }
}

using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.EclipseCosmosDb;

public sealed class EclipseDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public EclipseDbContext(DbContextOptions<EclipseDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EclipseDbContext).Assembly);
    }
}

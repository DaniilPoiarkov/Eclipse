using Eclipse.DataAccess.Model;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.CosmosDb;

public sealed class EclipseDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }


    private readonly IModelBuilderConfigurator _modelBuilderConfigurator;

    public EclipseDbContext(DbContextOptions<EclipseDbContext> options, IModelBuilderConfigurator modelBuilderConfigurator)
        : base(options)
    {
        _modelBuilderConfigurator = modelBuilderConfigurator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _modelBuilderConfigurator.Configure(modelBuilder);
    }
}

using Eclipse.DataAccess.Model;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.Cosmos;

internal sealed class EclipseDbContext : DbContext
{
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

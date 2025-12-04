using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Reflection;

namespace Eclipse.DataAccess.Migrations;

internal sealed class MigrationRunner<AppDbContext> : IMigrationRunner
    where AppDbContext : DbContext
{
    private readonly AppDbContext _context;

    private readonly IEnumerable<IMigration> _migrations;

    private readonly ILogger<MigrationRunner<AppDbContext>> _logger;

    private DbSet<VersionInfo> Set => _context.Set<VersionInfo>();

    public MigrationRunner(AppDbContext context, IEnumerable<IMigration> migrations, ILogger<MigrationRunner<AppDbContext>> logger)
    {
        _context = context;
        _migrations = migrations;
        _logger = logger;
    }

    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        var currentVersion = await Set.OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync(cancellationToken);

        var migrations = _migrations.Select(m => (Migration: m, Attribute: m.GetType().GetCustomAttribute<MigrationAttribute>()))
            .Where(pair => pair.Attribute is not null
                && (currentVersion is null || pair.Attribute.Version > currentVersion.Version))
            .OrderBy(pair => pair.Attribute!.Version)
            .ToList();

        if (migrations.IsNullOrEmpty())
        {
            _logger.LogInformation("No migrations to apply");
            return;
        }

        var versionInfos = new List<VersionInfo>(migrations.Count);

        foreach (var (migration, attribute) in migrations)
        {
            if (attribute is null)
            {
                throw new InvalidOperationException($"Migration {migration.GetType().FullName} is missing MigrationAttribute");
            }

            _logger.LogInformation("Applying migration: {Version}", attribute.Version);

            await migration.Migrate(cancellationToken).ConfigureAwait(false);

            var versionInfo = new VersionInfo
            {
                Id = Guid.CreateVersion7(),
                Version = attribute.Version,
                Description = attribute.Description,
                AppliedAt = DateTime.UtcNow,
            };

            versionInfos.Add(versionInfo);
        }

        await Set.AddRangeAsync(versionInfos, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

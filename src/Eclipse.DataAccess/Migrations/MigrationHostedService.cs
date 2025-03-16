using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Users;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Reflection;

namespace Eclipse.DataAccess.Migrations;

internal sealed class MigrationHostedService : IHostedService
{
    private static readonly int ItemsCount = 1;

    private readonly IEnumerable<IMigration> _migrations;

    private readonly ILogger<MigrationHostedService> _logger;

    private readonly CosmosClient _cosmosClient;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public MigrationHostedService(
        IEnumerable<IMigration> migrations,
        ILogger<MigrationHostedService> logger,
        CosmosClient cosmosClient,
        IOptions<CosmosDbContextOptions> options)
    {
        _migrations = migrations;
        _logger = logger;
        _cosmosClient = cosmosClient;
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = _options.Value;
        var container = _cosmosClient.GetContainer(options.DatabaseId, options.Container);

        var query = 
            """
                select top 1 * from c
                where c.Discriminator = 'VersionInfo'
                order by c.Version desc
            """;

        var iterator = container.GetItemQueryIterator<VersionInfo>(query,
            requestOptions: new QueryRequestOptions
            {
                MaxItemCount = ItemsCount,
            });

        var currentVersion = (await iterator.ReadNextAsync(cancellationToken)).FirstOrDefault();

        var migrations = _migrations.Select(m => (Migration: m, Attribute: m.GetType().GetCustomAttribute<MigrationAttribute>()))
            .Where(pair => pair.Attribute is not null
                && (currentVersion is null || pair.Attribute.Version > currentVersion.Version))
            .OrderBy(pair => pair.Attribute!.Version)
            .ToList();

        var versionUploads = new List<Task>(migrations.Count);

        foreach (var (migration, attribute) in migrations)
        {
            _logger.LogInformation("Applying migration: {Version}", attribute!.Version);

            await migration.Migrate(cancellationToken).ConfigureAwait(false);

            versionUploads.Add(
                container.CreateItemAsync(
                    new
                    {
                        id = Guid.NewGuid(),
                        Discriminator = nameof(VersionInfo),
                        AppliedAt = DateTime.UtcNow,
                        attribute.Version,
                        attribute.Description,
                    },
                    requestOptions: new ItemRequestOptions(),
                    cancellationToken: cancellationToken
                )
            );
        }

        await Task.WhenAll(versionUploads);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eclipse.Core.Stores.Cosmos;

internal sealed class CosmosStoresInitializerHostedService : IHostedService
{
    private readonly CosmosClient _client;

    private readonly ILogger<CosmosStoresInitializerHostedService> _logger;

    private readonly IOptions<EclipseCoreStoresCosmosOptions> _options;

    public CosmosStoresInitializerHostedService(
        CosmosClient client,
        ILogger<CosmosStoresInitializerHostedService> logger,
        IOptions<EclipseCoreStoresCosmosOptions> options)
    {
        _client = client;
        _logger = logger;
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Value.CreateDatabaseAndContainerIfNotExists)
        {
            _logger.LogInformation("Initialization of stores database and container is skipped.");
            return;
        }

        _logger.LogInformation($"Initializing stores. Database: {_options.Value.Database}, container: {_options.Value.Container}, partition key: {_options.Value.PartitionKey}.");

        var database = await _client.CreateDatabaseIfNotExistsAsync(
            _options.Value.Database,
            ThroughputProperties.CreateManualThroughput(_options.Value.Throughput),
            cancellationToken: cancellationToken
        );

        await database.Database.CreateContainerIfNotExistsAsync(
            _options.Value.Container,
            _options.Value.PartitionKey,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation("Initializing stores finished successfully.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


using Eclipse.DataAccess.Cosmos;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Migrations;

[Migration(20250316003000, "test")]
internal sealed class _20250316003000_TestMigration : IMigration
{
    private readonly CosmosClient _client;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public _20250316003000_TestMigration(CosmosClient client, IOptions<CosmosDbContextOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        var container = _client.GetContainer(_options.Value.DatabaseId, _options.Value.Container);

        await container.CreateItemAsync(new
        {
            id = Guid.CreateVersion7(),
            Discriminator = "TEST",
            Value = "Well..some test entity for migraiton"
        });
    }
}

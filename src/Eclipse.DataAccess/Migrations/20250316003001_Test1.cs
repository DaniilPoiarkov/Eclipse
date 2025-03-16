using Eclipse.DataAccess.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Migrations;

[Migration(20250316003001)]
internal sealed class _20250316003001_Test1 : IMigration
{
    private readonly CosmosClient _client;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public _20250316003001_Test1(CosmosClient client, IOptions<CosmosDbContextOptions> options)
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
            Value = "Well..some test entity for one more migraiton"
        });
    }
}

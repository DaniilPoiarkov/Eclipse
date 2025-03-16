using Eclipse.DataAccess.Cosmos;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Eclipse.DataAccess.Migrations;

[Migration(20250316003002)]
internal sealed class _20250316003002_Test2 : IMigration
{
    private readonly CosmosClient _client;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public _20250316003002_Test2(CosmosClient client, IOptions<CosmosDbContextOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task Migrate(CancellationToken cancellationToken = default)
    {
        var container = _client.GetContainer(_options.Value.DatabaseId, _options.Value.Container);

        var iterator = container.GetItemQueryIterator<Test>("""select * from c where c.Discriminator = 'TEST'""");

        var items = await iterator.ReadNextAsync(cancellationToken);

        var tasks = new List<Task>();

        foreach (var i in items)
        {
            i.IsEnabled = true;

            tasks.Add(container.UpsertItemAsync(i, cancellationToken: cancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    internal class Test
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string? Value { get; set; }
        public bool IsEnabled { get; set; }
    }
}

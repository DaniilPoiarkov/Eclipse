using Eclipse.DataAccess.Cosmos;

using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

using User = Eclipse.Domain.Users.User;

namespace Eclipse.DataAccess.Migrations.Migrations.Users;

[Migration(20250317013000, "Set default CreatedAt and IsEnabled properties for existing documents")]
internal sealed class _20250317013000_DefaultValuesForUser : Migration
{
    private readonly CosmosClient _cosmosClient;

    private readonly IOptions<CosmosDbContextOptions> _options;

    public _20250317013000_DefaultValuesForUser(CosmosClient cosmosClient, IOptions<CosmosDbContextOptions> options)
    {
        _cosmosClient = cosmosClient;
        _options = options;
    }

    public async override Task Migrate(CancellationToken cancellationToken = default)
    {
        var container = _cosmosClient.GetContainer(_options.Value.DatabaseId, _options.Value.Container);

        using var documentsWithoutCreatedAtProperty = container.GetItemQueryIterator<DocumentId>(
            "select c.Id, c.id as CosmosId from c where not is_defined(c.CreatedAt) and c.Discriminator = 'User'"
        );

        using var documentsWithoutIsEnabledProperty = container.GetItemQueryIterator<DocumentId>(
            "select c.Id, c.id as CosmosId from c where not is_defined(c.IsEnabled) and c.Discriminator = 'User'"
        );

        var createdAtUpdating = UpdateAsync(
            documentsWithoutCreatedAtProperty,
            PatchOperation.Set($"/{nameof(User.CreatedAt)}", DateTime.UtcNow),
            container,
            cancellationToken
        );

        var isEnabledUpdating = UpdateAsync(
            documentsWithoutIsEnabledProperty,
            PatchOperation.Set($"/{nameof(User.IsEnabled)}", true),
            container,
            cancellationToken
        );

        await Task.WhenAll(createdAtUpdating, isEnabledUpdating);
    }
}

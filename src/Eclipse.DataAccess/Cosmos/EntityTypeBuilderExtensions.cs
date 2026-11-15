using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eclipse.DataAccess.Cosmos;

internal static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Uses the raw <c>Id</c> value as the Cosmos <c>id</c> (without the discriminator prefix)
    /// while still persisting it as the <c>Id</c> JSON property, which is the container partition key path (<c>/Id</c>).
    /// </summary>
    public static EntityTypeBuilder HasKeyAsJsonId(this EntityTypeBuilder builder)
    {
        builder.Property(CosmosDocumentConsts.PartitionKeyProperty)
            .ToJsonProperty(CosmosDocumentConsts.PartitionKeyProperty);

        builder.HasDiscriminatorInJsonId(false);

        return builder;
    }
}

namespace Eclipse.DataAccess.Cosmos;

internal static class CosmosDocumentConsts
{
    public const string PartitionKeyProperty = "Id";

    public const string IdProperty = "id";

    public const string DiscriminatorProperty = "$type";

    /// <summary>
    /// Discriminator JSON property used before EF Core 11.
    /// </summary>
    public const string LegacyDiscriminatorProperty = "Discriminator";
}

namespace Eclipse.DataAccess.CosmosDb;

public sealed class CosmosDbContextOptions
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseId { get; set; } = null!;

    public string Endpoint { get; set; } = null!;
}

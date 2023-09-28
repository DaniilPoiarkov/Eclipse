namespace Eclipse.DataAccess.CosmosDb;

public class CosmosDbContextOptions
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseId { get; set; } = null!;
}

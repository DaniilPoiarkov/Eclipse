using Eclipse.DataAccess.CosmosDb;

namespace Eclipse.DataAccess.Builder;

public class DataAccessModuleBuilder
{
    public CosmosDbContextOptions CosmosOptions { get; set; } = new();
}

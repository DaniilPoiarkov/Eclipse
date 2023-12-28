using Eclipse.DataAccess.CosmosDb;

namespace Eclipse.DataAccess.Builder;

[Serializable]
public sealed class DataAccessModuleBuilder
{
    public CosmosDbContextOptions CosmosOptions { get; set; } = new();
}

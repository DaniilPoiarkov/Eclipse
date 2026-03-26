namespace Eclipse.Core.Stores.Cosmos;

/// <summary>
/// Specifies connection type to cosmos database.
/// </summary>
public enum CosmosConnectionType
{
    /// <summary>
    /// Uses ConnectionString to connect to database.
    /// </summary>
    ConnectionString,

    /// <summary>
    /// Uses AccountEndoint and <a cref="Azure.Identity.DefaultAzureCredential"></a> to connect to the database.
    /// </summary>
    ManagedIdentity
}

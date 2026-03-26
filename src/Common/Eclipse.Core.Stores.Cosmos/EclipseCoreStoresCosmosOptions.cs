namespace Eclipse.Core.Stores.Cosmos;

public sealed class EclipseCoreStoresCosmosOptions
{
    /// <summary>
    /// Gets the account endpoint.
    /// </summary>
    public string AccountEndpoint { get; init; } = string.Empty;

    /// <summary>
    /// Gets the database.
    /// </summary>
    public required string Database { get; init; }

    /// <summary>
    /// Specifies container name for stores.
    /// </summary>
    public required string Container { get; init; }

    /// <summary>
    /// Gets the connection string. Used only when <a cref="ConnectionType"></a> is set to <a cref="CosmosConnectionType.ConnectionString"></a>
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Allows to specify partition key for the container. If not set, container will be created with /id as partition key
    /// </summary>
    public string PartitionKey { get; init; } = "/id";

    /// <summary>
    /// If set to <a cref="true"></a> will create database and container if they do not exist with specified <a cref="PartitionKey"></a>.
    /// </summary>
    public bool CreateDatabaseAndContainerIfNotExists { get; init; } = true;

    /// <summary>
    /// If <a cref="CreateDatabaseAndContainerIfNotExists"></a> is set to <a cref="true"></a>, specifies throughput for the database. Default is 1000 RU/s.
    /// </summary>
    public int Throughput { get; set; } = 1000;

    /// <summary>
    /// Specifies type of connection. Default is <a cref="CosmosConnectionType.ConnectionString"></a>
    /// </summary>
    public CosmosConnectionType ConnectionType { get; init; } = CosmosConnectionType.ConnectionString;
}

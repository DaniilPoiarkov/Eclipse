namespace Eclipse.Core.Stores.Cosmos;

/// <summary>
/// Allows to enrich documents before they are stored in Cosmos DB. This can be used to add additional metadata or perform transformations on the document.
/// </summary>
public interface IDocumentEnricher
{
    void Enrich(IDictionary<string, object?> properties);
}

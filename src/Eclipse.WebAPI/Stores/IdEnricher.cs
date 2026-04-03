using Eclipse.Core.Stores.Cosmos;

internal sealed class IdEnricher : IDocumentEnricher
{
    public void Enrich(IDictionary<string, object?> properties)
    {
        if (properties.ContainsKey("Id"))
        {
            return;
        }

        properties["Id"] = Guid.CreateVersion7();
    }
}

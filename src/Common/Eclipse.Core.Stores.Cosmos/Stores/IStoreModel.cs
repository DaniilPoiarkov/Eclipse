namespace Eclipse.Core.Stores.Cosmos.Stores;

internal interface IStoreModel
{
    Dictionary<string, object?> ToDictionary();

    string Discriminator { get; }

    string Id { get; }
}

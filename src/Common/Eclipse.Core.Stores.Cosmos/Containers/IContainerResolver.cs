using Microsoft.Azure.Cosmos;

namespace Eclipse.Core.Stores.Cosmos.Containers;

internal interface IContainerResolver
{
    Container Container { get; }
}

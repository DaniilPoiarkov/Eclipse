using Eclipse.Core.Stores;
using Eclipse.Core.Stores.InMemory;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Core.Tests.Stores;

public sealed class InMemoryPipelineStoreTests : PipelineStoreTests
{
    protected internal override IPipelineStore Initialize()
    {
        var sp = new ServiceCollection()
            .AddTransient<TestPipeline>()
            .BuildServiceProvider();

        return new InMemoryPipelineStore(sp);
    }
}

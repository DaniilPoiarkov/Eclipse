using Eclipse.Core.Stores;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Pipelines.Tests.Stores;

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

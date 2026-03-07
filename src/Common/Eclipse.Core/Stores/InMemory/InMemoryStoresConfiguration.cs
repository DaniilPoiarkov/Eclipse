using Eclipse.Core.Builder;

namespace Eclipse.Core.Stores.InMemory;

public static class InMemoryStoresConfiguration
{
    /// <summary>
    /// Registeres in-memory implementations of the pipeline and message stores.
    /// This is useful for testing or scenarios where persistence is not required, but not sutable for production usage.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static CoreBuilder UseInMemoryStores(this CoreBuilder builder)
    {
        return builder.UsePipelineStore<InMemoryPipelineStore>()
            .UseMessageStore<InMemoryMessageStore>();
    }
}

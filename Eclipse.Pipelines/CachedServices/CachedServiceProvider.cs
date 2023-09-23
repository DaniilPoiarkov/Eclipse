using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Pipelines.CachedServices;

internal static class CachedServiceProvider
{
    public static IServiceProvider Services { get; private set; } = new ServiceCollection().BuildServiceProvider();

    internal static void SetServiceProvider(IServiceProvider serviceProvider) =>
        Services = serviceProvider;
}

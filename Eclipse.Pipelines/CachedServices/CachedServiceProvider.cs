namespace Eclipse.Pipelines.CachedServices;

internal static class CachedServiceProvider
{
    public static IServiceProvider? Services { get; private set; }

    internal static void SetServiceProvider(IServiceProvider serviceProvider) => Services = serviceProvider;
}

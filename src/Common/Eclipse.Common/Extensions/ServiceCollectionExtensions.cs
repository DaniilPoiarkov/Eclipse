using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IConfiguration GetConfiguration(this IServiceCollection services)
    {
        return services.GetConfigurationOrNull()
            ?? throw new ApplicationException(
                $"Cannot find an implementation of {typeof(IConfiguration).AssemblyQualifiedName} in the service collection."
            );
    }

    private static IConfiguration? GetConfigurationOrNull(this IServiceCollection services)
    {
        var hostBuilder = services.GetSingletonInstanceOrNull<HostBuilderContext>();

        if (hostBuilder?.Configuration is not null)
        {
            return hostBuilder.Configuration;
        }

        return services.GetSingletonInstanceOrNull<IConfiguration>();
    }

    private static T? GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T?)services
            .FirstOrDefault(s => s.ServiceType == typeof(T))?
            .NormalizedImplementationInstance();
    }

    private static object? NormalizedImplementationInstance(this ServiceDescriptor serviceDescriptor)
    {
        return serviceDescriptor.IsKeyedService
            ? serviceDescriptor.KeyedImplementationInstance
            : serviceDescriptor.ImplementationInstance;
    }
}

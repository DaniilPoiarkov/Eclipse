using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Application.Contracts;

/// <summary>
/// Takes responsibility to provide public API to use Application Module
/// </summary>
public static class EclipseApplicationContractsModule
{
    public static IServiceCollection AddApplicationContractsModule(this IServiceCollection services)
    {
        return services;
    }
}

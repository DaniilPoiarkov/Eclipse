using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.DataAccess;

/// <summary>
/// Responsible for data access
/// </summary>
public static class EclipseDataAccessModule
{
    public static IServiceCollection AddDataAccessModule(this IServiceCollection services)
    {
        // TODO: add DbContext etc.
        return services;
    }
}

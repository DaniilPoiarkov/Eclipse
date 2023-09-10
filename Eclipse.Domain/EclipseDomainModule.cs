using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Domain;

/// <summary>
/// Takes responsibility for domain logic
/// </summary>
public static class EclipseDomainModule
{
    public static IServiceCollection AddDomainModule(this IServiceCollection services)
    {
        return services;
    }
}

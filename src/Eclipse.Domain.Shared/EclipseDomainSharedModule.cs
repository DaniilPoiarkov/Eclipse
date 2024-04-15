using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Domain.Shared;

/// <summary>
/// Takes responsibility for public accessible data used for domain module (e.g. enums/constants)
/// </summary>
public static class EclipseDomainSharedModule
{
    public static IServiceCollection AddDomainSharedModule(this IServiceCollection services)
    {
        return services;
    }
}

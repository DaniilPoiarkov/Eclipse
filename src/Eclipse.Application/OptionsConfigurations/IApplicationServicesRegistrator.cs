using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Application.OptionsConfigurations;

internal interface IApplicationServicesRegistrator
{
    IServiceCollection Register(IServiceCollection services);
}

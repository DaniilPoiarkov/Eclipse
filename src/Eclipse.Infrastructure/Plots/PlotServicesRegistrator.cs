using Eclipse.Common.Plots;
using Eclipse.Infrastructure.Plots.TickGenerators;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Infrastructure.Plots;

internal static class PlotServicesRegistrator
{
    internal static IServiceCollection AddPlotServices(this IServiceCollection services)
    {
        services.AddScoped<IPlotTemplateFactory, PlotTemplateFactory>()
            .AddScoped<IPlotTemplateFactory, PlotTemplateFactory>()
            .AddScoped<ITickGeneratorFactory<DateTime>, DateTimeTickGeneratorFactory>()
            .AddScoped<IPlotGenerator, PlotGenerator>();

        return services;
    }
}

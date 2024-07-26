using Eclipse.Localization.Builder;
using Eclipse.Localization.Culture;
using Eclipse.Localization.Localizers;
using Eclipse.Localization.Resources;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Eclipse.Localization;

public static class LocalizationModule
{
    /// <summary>
    /// Registers <a cref="IStringLocalizer{T}"></a> and <a cref="ICurrentCulture"></a> to use localization.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLocalization(this IServiceCollection services, Action<ILocalizationBuilder> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<LocalizationBuilder>(configuration.Invoke);

        services.AddHttpContextAccessor();

        services.AddSingleton<IResourceProvider, ResourceProvider>();

        services.RemoveAll<IStringLocalizerFactory>()
            .AddSingleton<JsonStringLocalizerFactory>()
            .AddSingleton<IStringLocalizerFactory>(sp => sp.GetRequiredService<JsonStringLocalizerFactory>());

        services.AddTransient(sp => sp.GetRequiredService<JsonStringLocalizerFactory>().Create());

        services.RemoveAll(typeof(IStringLocalizer<>))
            .AddTransient(typeof(IStringLocalizer<>), typeof(TypedJsonStringLocalizer<>));

        services.AddScoped<CurrentCulture>()
            .AddScoped<ICurrentCulture>(sp => sp.GetRequiredService<CurrentCulture>());

        services.AddScoped<CultureResolverMiddleware>();

        return services;
    }

    public static WebApplication UseLocalization(this WebApplication app)
    {
        app.UseMiddleware<CultureResolverMiddleware>();
        return app;
    }
}

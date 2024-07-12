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
    /// Registers <a cref="ILocalizer"></a> with provided configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLocalization(this IServiceCollection services, Action<ILocalizationBuilder>? configuration = null)
    {
        var builder = new LocalizationBuilder();

        configuration?.Invoke(builder);
        var localizer = builder.Build();

        services.AddSingleton(localizer);

        return services;
    }

    public static IServiceCollection AddLocalizationV2(this IServiceCollection services, Action<ILocalizationBuilder> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        services.Configure<LocalizationBuilderV2>(configuration.Invoke);

        services.AddSingleton<IResourceProvider, ResourceProvider>();

        services.RemoveAll<IStringLocalizerFactory>()
            .AddSingleton<JsonStringLocalizerFactory>()
            .AddSingleton<IStringLocalizerFactory>(sp => sp.GetRequiredService<JsonStringLocalizerFactory>())
            .AddSingleton<ILocalizerFactory>(sp => sp.GetRequiredService<JsonStringLocalizerFactory>());

        services.AddTransient(sp => sp.GetRequiredService<JsonStringLocalizerFactory>().Create());

        services.RemoveAll(typeof(IStringLocalizer<>))
            .AddTransient(typeof(IStringLocalizer<>), typeof(TypedJsonStringLocalizer<>))
            .AddTransient(typeof(IStringLocalizer<>), typeof(TypedJsonStringLocalizer<>))
            .AddTransient(sp => sp.GetRequiredService<ILocalizerFactory>().Create());

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

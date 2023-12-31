﻿using Eclipse.Localization.Builder;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Localization;

public static class LocalizationModule
{
    /// <summary>
    /// Registers <a cref="Localizers.ILocalizer"></a> with provided configuration
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
}

using MassTransit;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Notifications;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, Action<NotificationsModuleOptions> options)
    {
        services.Configure(options);

        services.AddMassTransit(bus =>
        {
            bus.AddConsumers(typeof(NotificationsModule).Assembly);

            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingInMemory((context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

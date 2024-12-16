using MassTransit;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Notifications;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.UsingInMemory((context, configurator) =>
            {
                configurator.ConfigureEndpoints(context);
            });

            bus.AddConsumers(typeof(NotificationsModule).Assembly);
        });

        return services;
    }
}

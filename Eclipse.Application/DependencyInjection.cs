using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Stores;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Pipelines;
using Eclipse.Application.Telegram.Stores;
using Eclipse.Core;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddEclipseCore()
            .AddTransient<IUserStore, UserStore>()
            .AddTransient<IPipelineStore, PipelineStore>()
            .AddTransient<ITelegramUpdateHandler, TelegramUpdateHandler>();

        services.Replace(ServiceDescriptor.Transient<INotFoundPipeline, EclipseNotFoundPipeline>());

        services.Scan(tss => tss.FromAssemblyOf<TelegramUpdateHandler>()
            .AddClasses(c => c.AssignableTo<PipelineBase>())
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());

        return services;
    }
}

using Eclipse.Core.Keywords;
using Eclipse.Core.Stores;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Core.Builder;

/// <summary>
/// Builder for Core module.
/// </summary>
public sealed class CoreBuilder
{
    private readonly IServiceCollection _services;

    public CoreBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Allows decorate pipeline stage run in manner of middleware.
    /// </summary>
    /// <typeparam name="TPipelineExecutionDecorator">The type of the pipeline execution decorator.</typeparam>
    /// <returns></returns>
    public CoreBuilder Decorate<TPipelineExecutionDecorator>()
        where TPipelineExecutionDecorator : class, IPipelineExecutionDecorator
    {
        _services.AddTransient<IPipelineExecutionDecorator, TPipelineExecutionDecorator>();
        return this;
    }

    /// <summary>
    /// <para>
    /// Allows you to map keywords before attempting to retrieve pipeline when the update type is <a cref="Telegram.Bot.Types.Enums.UpdateType.Message"></a>.
    /// </para>
    /// <para>
    /// Registers only first implementation, if no provided uses default.
    /// </para>
    /// </summary>
    /// <typeparam name="TKeywordMapper">The type of the keyword mapper.</typeparam>
    /// <param name="lifetime">The lifetime.</param>
    /// <returns></returns>
    public CoreBuilder UseKeywordMapper<TKeywordMapper>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TKeywordMapper : class, IKeywordMapper
    {
        _services.TryAdd(new ServiceDescriptor(typeof(IKeywordMapper), typeof(TKeywordMapper), lifetime));
        return this;
    }

    /// <summary>
    /// Registeres implementation of <a href="IPipelineStore"/> to store pipelines.
    /// By default, no implementation is registered, so you need to register it yourself.
    /// If no implementation is registered, the pipeline will be stored in memory, which is not recommended for production use.
    /// </summary>
    /// <typeparam name="TPipelineStore">The type of the pipeline store.</typeparam>
    /// <returns></returns>
    public CoreBuilder UsePipelineStore<TPipelineStore>()
        where TPipelineStore : class, IPipelineStore
    {
        _services.AddScoped<IPipelineStore, TPipelineStore>();
        return this;
    }

    /// <summary>
    /// Registeres implementation of <a href="IMessageStore"/> to store messages.
    /// By default, no implementation is registered, so you need to register it yourself.
    /// If no implementation is registered, the pipeline will be stored in memory, which is not recommended for production use.
    /// </summary>
    /// <typeparam name="TPipelineStore">The type of the pipeline store.</typeparam>
    /// <returns></returns>
    public CoreBuilder UseMessageStore<TMessageStore>()
        where TMessageStore : class, IMessageStore
    {
        _services.AddScoped<IMessageStore, TMessageStore>();
        return this;
    }

    /// <summary>
    /// Configures the options.
    /// </summary>
    /// <param name="configure">The configure.</param>
    /// <returns></returns>
    public CoreBuilder ConfigureOptions(Action<CoreOptions> configure)
    {
        _services.Configure(configure);
        return this;
    }
}

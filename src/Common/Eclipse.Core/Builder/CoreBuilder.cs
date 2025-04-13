using Eclipse.Core.Keywords;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Core.Builder;

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
}

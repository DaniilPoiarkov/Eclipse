using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider.Handlers;
using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider;

internal sealed class PipelineProvider : IPipelineProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IAccessDeniedPipeline _accessDeniedPipeline;

    private readonly INotFoundPipeline _notFoundPipeline;

    private readonly Dictionary<UpdateType, IRouteHandler> _handlers;

    public PipelineProvider(
        IServiceProvider serviceProvider,
        IAccessDeniedPipeline accessDeniedPipeline,
        INotFoundPipeline notFoundPipeline,
        IEnumerable<IRouteHandler> handlers)
    {
        _serviceProvider = serviceProvider;
        _accessDeniedPipeline = accessDeniedPipeline;
        _notFoundPipeline = notFoundPipeline;
        _handlers = handlers.ToDictionary(h => h.Type);
    }

    public IPipeline Get(Update update)
    {
        if (!_handlers.TryGetValue(update.Type, out var handler))
        {
            handler = new UnknownHandler();
        }

        var pipeline = handler.Get(update) ?? _notFoundPipeline;

        pipeline.SetUpdate(update);

        return ValidOrAccessDenied(pipeline, update);
    }

    private IPipeline ValidOrAccessDenied(IPipeline pipeline, Update update)
    {
        if (IsContextValid(pipeline, update, out var results))
        {
            return pipeline;
        }

        _accessDeniedPipeline.SetResults(results);

        return _accessDeniedPipeline;
    }

    private bool IsContextValid(IPipeline pipeline, Update update, out ValidationResult[] results)
    {
        var validationAttributes = pipeline.GetType()
            .GetCustomAttributes<ContextValidationAttribute>()
            .ToList();

        if (validationAttributes.Count == 0)
        {
            results = [];
            return true;
        }

        using var scope = _serviceProvider.CreateScope();

        var context = new ValidationContext(scope.ServiceProvider, update);

        results = [.. validationAttributes.Select(a => a.Validate(context))];

        return results.All(result => result.IsSucceded);
    }
}

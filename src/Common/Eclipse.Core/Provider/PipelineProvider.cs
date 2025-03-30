using Eclipse.Core.Pipelines;
using Eclipse.Core.Provider.Handlers;
using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider;

internal sealed class PipelineProvider : IPipelineProviderV2
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IEnumerable<PipelineBase> _pipelines;

    private readonly Dictionary<UpdateType, IProviderHandler> _handlers;

    public PipelineProvider(IServiceProvider serviceProvider, IEnumerable<PipelineBase> pipelines, IEnumerable<IProviderHandler> handlers)
    {
        _serviceProvider = serviceProvider;
        _pipelines = pipelines;
        _handlers = handlers.ToDictionary(h => h.Type);
    }

    public PipelineBase Get(Update update)
    {
        if (!_handlers.TryGetValue(update.Type, out var handler))
        {
            handler = new UnknownHandler();
        }

        var pipeline = handler.Get(update)
            ?? GetNotFoundPipeline();

        pipeline.Update = update;

        return ValidOrAccessDenied(pipeline, update);
    }

    private PipelineBase GetNotFoundPipeline()
    {
        return _pipelines.FirstOrDefault(p => p is INotFoundPipeline)
            ?? new NotFoundPipeline();
    }

    private PipelineBase ValidOrAccessDenied(PipelineBase pipeline, Update update)
    {
        if (IsContextValid(pipeline, update, out var results))
        {
            return pipeline;
        }

        if (_pipelines.FirstOrDefault(p => p is IAccessDeniedPipeline) is not IAccessDeniedPipeline accessDeniedPipeline)
        {
            return GetNotFoundPipeline();
        }

        accessDeniedPipeline.SetResults(results);

        return (PipelineBase)accessDeniedPipeline;
    }

    private bool IsContextValid(PipelineBase pipeline, Update update, out ValidationResult[] results)
    {
        var validationAttributes = pipeline.GetType()
            .GetCustomAttributes<ContextValidationAttribute>()
            .ToList();

        if (validationAttributes.Count == 0)
        {
            results = [];
            return true;
        }

        using var scope = _serviceProvider.CreateAsyncScope();

        var context = new ValidationContext(scope.ServiceProvider, update);

        results = [.. validationAttributes.Select(a => a.Validate(context))];

        return results.All(result => result.IsSucceded);
    }
}

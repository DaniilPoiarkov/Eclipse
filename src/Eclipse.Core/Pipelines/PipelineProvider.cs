using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Eclipse.Core.Pipelines;

internal sealed class PipelineProvider : IPipelineProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<RouteAttribute, PipelineBase> _pipelines;

    private readonly ICurrentTelegramUser _currentUser;

    public PipelineProvider(IEnumerable<PipelineBase> pipelines, IServiceProvider serviceProvider, ICurrentTelegramUser currentUser)
    {
        _pipelines = pipelines
            .Select(p => (Pipeline: p, Attribute: p.GetType().GetCustomAttribute<RouteAttribute>()))
            .Where(tuple => tuple.Attribute is not null)
            .ToDictionary(tuple => tuple.Attribute!, tuple => tuple.Pipeline);

        _serviceProvider = serviceProvider;
        _currentUser = currentUser;
    }

    public PipelineBase Get(string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            return GetNotFoundPipeline();
        }

        if (route.StartsWith('/'))
        {
            return GetByCommand(route);
        }

        return GetByRoute(route);
    }

    private PipelineBase GetByRoute(string route)
    {
        var pipeline = _pipelines.FirstOrDefault(p => p.Key.Route.Equals(route)).Value
            ?? GetNotFoundPipeline();

        return ValidOrAccessDenied(pipeline);
    }

    private PipelineBase GetByCommand(string command)
    {
        var pipeline = _pipelines.FirstOrDefault(kv => kv.Key.Command is not null && kv.Key.Command.Equals(command)).Value
            ?? GetNotFoundPipeline();

        return ValidOrAccessDenied(pipeline);
    }

    private PipelineBase GetNotFoundPipeline()
    {
        return _pipelines.First(kv => kv.Value is INotFoundPipeline).Value;
    }

    private PipelineBase ValidOrAccessDenied(PipelineBase pipeline)
    {
        if (Validate(pipeline, out var results))
        {
            return pipeline;
        }

        var accessDeniedPipeline = _pipelines.First(kv => kv.Value is IAccessDeniedPipeline).Value as IAccessDeniedPipeline;

        accessDeniedPipeline!.SetResults(results);

        return (accessDeniedPipeline as PipelineBase)!;
    }

    private bool Validate(PipelineBase pipeline, out IEnumerable<ValidationResult> results)
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

        var context = new ValidationContext(scope.ServiceProvider, _currentUser.GetCurrentUser());

        results = validationAttributes.Select(a => a.Validate(context));

        return results.All(result => result.IsSucceded);
    }
}

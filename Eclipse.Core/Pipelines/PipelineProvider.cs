using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Eclipse.Core.Pipelines;

internal class PipelineProvider : IPipelineProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<RouteAttribute, Type> _pipelines;

    public PipelineProvider(IEnumerable<PipelineBase> pipelines, IServiceProvider serviceProvider)
    {
        _pipelines = pipelines
            .Select(p => p.GetType())
            .Where(type => type.GetCustomAttribute<RouteAttribute>() is not null)
            .ToDictionary(pipeline => pipeline.GetCustomAttribute<RouteAttribute>()!);

        _serviceProvider = serviceProvider;
    }

    public PipelineBase Get(string route)
    {
        if (string.IsNullOrEmpty(route))
        {
            var pipeline = _serviceProvider.GetRequiredService<INotFoundPipeline>() as PipelineBase;
            return pipeline!;
        }

        if (route.StartsWith("/"))
        {
            return GetByCommand(route);
        }

        return ReturnByRoute(route);
    }

    private PipelineBase ReturnByRoute(string route)
    {
        var pipelineType = _pipelines.FirstOrDefault(p => p.Key.Route.Equals(route)).Value
            ?? typeof(INotFoundPipeline);

        return ResolveAndValidate(pipelineType);
    }

    private PipelineBase GetByCommand(string route)
    {
        var pipelineType = _pipelines.FirstOrDefault(kv => kv.Key.Command is not null && kv.Key.Command.Equals(route)).Value
            ?? typeof(INotFoundPipeline);

        return ResolveAndValidate(pipelineType);
    }

    private PipelineBase ResolveAndValidate(Type pipelineType)
    {
        var results = Validate(pipelineType!, out var isValid);

        if (!isValid)
        {
            var accessDeniedPipeline = _serviceProvider.GetRequiredService<IAccessDeniedPipeline>();
            accessDeniedPipeline.SetResults(results);
            return (accessDeniedPipeline as PipelineBase)!;
        }

        return (_serviceProvider.GetRequiredService(pipelineType) as PipelineBase)!;
    }

    private IEnumerable<ValidationResult> Validate(Type pipeline, out bool isValid)
    {
        var validationAttributes = pipeline.GetCustomAttributes<ContextValidationAttribute>().ToList();

        if (validationAttributes.Count == 0)
        {
            isValid = true;
            return Enumerable.Empty<ValidationResult>();
        }

        var context = new ValidationContext(_serviceProvider);

        var validationResult = validationAttributes.Select(a => a.Validate(context));

        isValid = validationResult.All(result => result.IsSucceded);
        return validationResult;
    }
}

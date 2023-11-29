using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Core.Validation;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace Eclipse.Core.Pipelines;

internal sealed class PipelineProvider : IPipelineProvider
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<RouteAttribute, Type> _pipelines;

    private readonly ICurrentTelegramUser _currentUser;

    public PipelineProvider(IEnumerable<PipelineBase> pipelines, IServiceProvider serviceProvider, ICurrentTelegramUser currentUser)
    {
        _pipelines = pipelines
            .Select(p => p.GetType())
            .Where(type => type.GetCustomAttribute<RouteAttribute>() is not null)
            .ToDictionary(pipeline => pipeline.GetCustomAttribute<RouteAttribute>()!);

        _serviceProvider = serviceProvider;
        _currentUser = currentUser;
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
        if (!Validate(pipelineType, out var results))
        {
            var accessDeniedPipeline = _serviceProvider.GetRequiredService<IAccessDeniedPipeline>();
            accessDeniedPipeline.SetResults(results);
            return (accessDeniedPipeline as PipelineBase)!;
        }

        return (_serviceProvider.GetRequiredService(pipelineType) as PipelineBase)!;
    }

    private bool Validate(Type pipeline, out IEnumerable<ValidationResult> results)
    {
        var validationAttributes = pipeline.GetCustomAttributes<ContextValidationAttribute>().ToList();

        if (validationAttributes.Count == 0)
        {
            results = Enumerable.Empty<ValidationResult>();
            return true;
        }

        var context = new ValidationContext(_serviceProvider, _currentUser.GetCurrentUser());

        results = validationAttributes.Select(a => a.Validate(context));

        return results.All(result => result.IsSucceded);
    }
}

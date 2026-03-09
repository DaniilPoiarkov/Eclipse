using Eclipse.Core.Validation;

namespace Eclipse.Core.Pipelines;

/// <summary>
/// Use this interface to override default implementation.
/// </summary>
public interface IAccessDeniedPipeline : IPipeline
{
    void SetResults(IEnumerable<ValidationResult> results);
}

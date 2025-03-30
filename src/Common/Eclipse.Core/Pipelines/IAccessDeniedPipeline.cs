using Eclipse.Core.Validation;

namespace Eclipse.Core.Pipelines;

/// <summary>
/// Use this interface to override default implementation. You also MUST inherit from <a cref="PipelineBase"></a> class.
/// </summary>
public interface IAccessDeniedPipeline
{
    void SetResults(IEnumerable<ValidationResult> results);
}

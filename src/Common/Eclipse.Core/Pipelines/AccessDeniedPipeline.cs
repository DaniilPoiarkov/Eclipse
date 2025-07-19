using Eclipse.Core.Context;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.Validation;

namespace Eclipse.Core.Pipelines;

[Route("", "/href_access_denied")]
public class AccessDeniedPipeline : PipelineBase, IAccessDeniedPipeline
{
    protected IReadOnlyList<ValidationResult> Errors { get; private set; } = [];

    public virtual void SetResults(IEnumerable<ValidationResult> results)
    {
        Errors = [.. results.Where(r => r.IsFailed)];
    }

    protected override void Initialize()
    {
        RegisterStage(SendErrors);
    }

    protected virtual IResult SendErrors(MessageContext context)
    {
        var errorMessages = Errors.Select(e => e.ErrorMessage);
        return Text(string.Join(Environment.NewLine, errorMessages));
    }
}

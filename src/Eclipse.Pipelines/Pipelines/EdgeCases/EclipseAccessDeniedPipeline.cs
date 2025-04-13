using Eclipse.Core.Context;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Results;
using Eclipse.Core.Routing;
using Eclipse.Core.Validation;

namespace Eclipse.Pipelines.Pipelines.EdgeCases;

[Route("", "/href_access_denied")]
internal sealed class EclipseAccessDeniedPipeline : EclipsePipelineBase, IAccessDeniedPipeline
{
    private IEnumerable<ValidationResult> _errors = [];

    public void SetResults(IEnumerable<ValidationResult> results)
    {
        _errors = results.Where(r => r.IsFailed);
    }

    protected override void Initialize()
    {
        RegisterStage(SendError);
    }

    private IResult SendError(MessageContext context)
    {
        var error = _errors
            .Where(e => !e.ErrorMessage.IsNullOrEmpty())
            .Select(e => Localizer[e.ErrorMessage!].Value)
            .Join(", ");

        if (error.IsNullOrEmpty())
        {
            return Text(Localizer["Pipelines:NotFound"]);
        }

        return Text(error);
    }
}

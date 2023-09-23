using Eclipse.Core.Core;
using Eclipse.Core.Validation;

namespace Eclipse.Pipelines.Pipelines.EdgeCases;

internal class EclipseAccessDeniedPipeline : EclipsePipelineBase, IAccessDeniedPipeline
{
    public void SetResults(IEnumerable<ValidationResult> results)
    { 
        // Just ignore. We don't let users know that this is existing command
    }

    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer["Pipelines:NotFound"]));
    }
}

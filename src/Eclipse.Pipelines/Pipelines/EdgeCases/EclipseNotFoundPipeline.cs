using Eclipse.Core.Pipelines;
using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.EdgeCases;

[Route("", "/href_not_found")]
internal sealed class EclipseNotFoundPipeline : EclipsePipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer["Pipelines:NotFound"]));
    }
}

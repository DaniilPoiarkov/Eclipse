using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.EdgeCases;

public sealed class EclipseNotFoundPipeline : EclipsePipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text(Localizer["Pipelines:NotFound"]));
    }
}

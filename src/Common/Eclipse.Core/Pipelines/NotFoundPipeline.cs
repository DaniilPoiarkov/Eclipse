using Eclipse.Core.Routing;

namespace Eclipse.Core.Pipelines;

[Route("", "")]
public class NotFoundPipeline : PipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text("Pipeline not found"));
    }
}

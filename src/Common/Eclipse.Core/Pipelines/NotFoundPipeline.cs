using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Core.Pipelines;

[Route("", "")]
public class NotFoundPipeline : PipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text("Pipeline not found"));
    }
}

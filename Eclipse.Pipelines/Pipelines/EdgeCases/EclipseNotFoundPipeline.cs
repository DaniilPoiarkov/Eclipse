using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Pipelines.EdgeCases;

public class EclipseNotFoundPipeline : PipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(_ => Text("Oh, I don't know what you want. Are you sure that you used buttons below?"));
    }
}

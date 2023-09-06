using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Pipelines;

public class EclipseNotFoundPipeline : PipelineBase, INotFoundPipeline
{
    protected override void Initialize()
    {
        RegisterStage(SendMessage);
    }

    public IResult SendMessage(MessageContext context) =>
        Text("Oh, I don't know what you want. Are you sure that you used buttons below?");
}

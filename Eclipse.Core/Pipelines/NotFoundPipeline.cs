using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Core.Pipelines;

[Route("", "")]
public class NotFoundPipeline : PipelineBase, INotFoundPipeline
{
    public IResult SendMessage(MessageContext context) => Text("Pipeline not found");

    protected override void Initialize()
    {
        RegisterStage(SendMessage);
    }
}

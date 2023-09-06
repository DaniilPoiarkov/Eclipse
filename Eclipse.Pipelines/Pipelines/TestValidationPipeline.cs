using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Attributes;

namespace Eclipse.Pipelines.Pipelines;

[TestValidation]
[Route("test", "/test")]
public class TestValidationPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(ctx => Text("Test"));
    }
}

using Eclipse.Application.Telegram.Attributes;
using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;

namespace Eclipse.Application.Telegram.Pipelines;

[TestValidation]
[Route("test", "/test")]
public class TestValidationPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(ctx => Text("Test"));
    }
}

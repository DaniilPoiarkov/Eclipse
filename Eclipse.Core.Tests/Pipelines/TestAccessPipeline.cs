using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Tests.Attributes;

namespace Eclipse.Core.Tests.Pipelines;

[Route("TestAccess", "/test_access")]
[ValidationFailed]
internal class TestAccessPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Empty());
    }
}

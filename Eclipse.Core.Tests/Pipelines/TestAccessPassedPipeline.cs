using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Tests.Attributes;

namespace Eclipse.Core.Tests.Pipelines;

[ValidationPassed]
[Route("TestAccessPassed", "/test_access_pass")]
internal class TestAccessPassedPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Empty());
    }
}

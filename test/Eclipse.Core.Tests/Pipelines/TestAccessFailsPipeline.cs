using Eclipse.Core.Pipelines;
using Eclipse.Core.Routing;
using Eclipse.Core.Tests.Attributes;

namespace Eclipse.Core.Tests.Pipelines;

[Route("TestAccessFails", "/test_access_fails")]
[ValidationFailed]
internal class TestAccessFailsPipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Empty());
    }
}

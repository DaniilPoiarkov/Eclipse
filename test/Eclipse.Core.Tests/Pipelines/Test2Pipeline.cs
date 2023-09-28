using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Tests.Pipelines;

[Route("Test2", "/test2")]
internal class Test2Pipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Empty());
        
        RegisterStage(_ =>
        {
            FinishPipeline();
            return Empty();
        });

        RegisterStage(_ => Empty());
    }
}

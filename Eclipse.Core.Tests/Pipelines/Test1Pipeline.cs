﻿using Eclipse.Core.Attributes;
using Eclipse.Core.Pipelines;

namespace Eclipse.Core.Tests.Pipelines;

[Route("Test1", "/test1")]
internal class Test1Pipeline : PipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Empty());
        RegisterStage(_ => Empty());
    }
}

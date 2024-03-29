﻿using Eclipse.Common.Cache;
using Eclipse.Core.Pipelines;

namespace Eclipse.Pipelines.Stores.Pipelines;

internal sealed class PipelineStore : StoreBase<PipelineBase, PipelineKey>, IPipelineStore
{
    public PipelineStore(ICacheService cacheService) : base(cacheService)
    {
    }
}

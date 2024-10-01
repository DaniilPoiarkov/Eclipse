﻿using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.MoodRecords;

namespace Eclipse.DataAccess.MoodRecords;

internal sealed class CachedMoodRecordRepository : CachedRepositoryBase<MoodRecord, IMoodRecordRepository>, IMoodRecordRepository
{
    public CachedMoodRecordRepository(IMoodRecordRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }
}

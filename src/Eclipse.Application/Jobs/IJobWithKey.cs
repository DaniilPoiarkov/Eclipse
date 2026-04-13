using Quartz;

namespace Eclipse.Application.Jobs;

internal interface IJobWithKey
{
    static abstract JobKey Key { get; }
}

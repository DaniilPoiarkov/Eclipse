using Quartz;

namespace Eclipse.Pipelines.Jobs;

internal abstract class EclipseJobBase : IJob
{
    public abstract Task Execute(IJobExecutionContext context);
}

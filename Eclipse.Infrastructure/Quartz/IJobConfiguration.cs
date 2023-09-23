using Quartz;

namespace Eclipse.Infrastructure.Quartz;

public interface IJobConfiguration
{
    IJobDetail BuildJob();

    ITrigger BuildTrigger();
}

using Quartz;

namespace Eclipse.Infrastructure.Quartz;

public interface IEclipseScheduler
{
    Task ScheduleJob(IJobConfiguration configuration);

    Task DeleteJob(JobKey key);

    Task Initialize();
}

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal interface IJobConfiguration
{
    void Schedule(IQuartzBuilder quartz);
}

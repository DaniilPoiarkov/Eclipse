using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal interface IJobConfiguration
{
    void Schedule(QuartzOptions options);
}

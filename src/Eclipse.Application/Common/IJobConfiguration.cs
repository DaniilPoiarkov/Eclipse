using Quartz;

namespace Eclipse.Application.Common;

internal interface IJobConfiguration
{
    void Configure(IServiceCollectionQuartzConfigurator configurator);
}

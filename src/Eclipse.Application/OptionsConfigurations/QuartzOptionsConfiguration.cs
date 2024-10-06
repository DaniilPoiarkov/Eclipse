using Eclipse.Application.OptionsConfigurations.Configurations;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal sealed class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        AddJob<ProcessOutboxMessagesJobConfiguration>(options);
        AddJob<DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration>(options);
        AddJob<ArchiveMoodRecordsJobConfiguration>(options);
    }

    private static void AddJob<TJobConfiguration>(QuartzOptions options)
        where TJobConfiguration : IJobConfiguration, new()
    {
        new TJobConfiguration().Schedule(options);
    }
}

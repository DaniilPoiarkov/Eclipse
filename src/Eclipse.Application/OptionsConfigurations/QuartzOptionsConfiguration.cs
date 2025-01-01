using Eclipse.Application.OptionsConfigurations.Configurations;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal sealed class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        AddJob<ConvertOutboxToInboxJobConfiguration>(options);
        AddJob<ProcessInboxMessagesJobConfiguration>(options); // TODO: Review
        AddJob<ProcessTypedInboxMessagesJobConfiguration>(options);
        AddJob<ProcessTypedInboxMessagesJobConfiguration>(options);
        AddJob<DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration>(options);
        AddJob<DeleteProcessedInboxMessagesJobConfiguration>(options);
        AddJob<ArchiveMoodRecordsJobConfiguration>(options);
    }

    private static void AddJob<TJobConfiguration>(QuartzOptions options)
        where TJobConfiguration : IJobConfiguration, new()
    {
        new TJobConfiguration().Schedule(options);
    }
}

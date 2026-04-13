using Eclipse.Application.InboxMessages;
using Eclipse.Application.MoodRecords.Archiving;
using Eclipse.Application.OutboxMessages;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal sealed class QuartzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        AddJob<ConvertOutboxToInboxJobConfiguration>(options);
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

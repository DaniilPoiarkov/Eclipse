using Eclipse.Application.ApiTokens.Expiration;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.MoodRecords.Archiving;
using Eclipse.Application.OutboxMessages;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations;

internal static class QuartzOptionsConfiguration
{
    public static void Configure(IQuartzBuilder quartz)
    {
        AddJob<ConvertOutboxToInboxJobConfiguration>(quartz);
        AddJob<ProcessTypedInboxMessagesJobConfiguration>(quartz);
        AddJob<DeleteSuccessfullyProcessedOutboxMessagesJobConfiguration>(quartz);
        AddJob<DeleteProcessedInboxMessagesJobConfiguration>(quartz);
        AddJob<ArchiveMoodRecordsJobConfiguration>(quartz);
        AddJob<NotifyExpiringApiTokensJobConfiguration>(quartz);
    }

    private static void AddJob<TJobConfiguration>(IQuartzBuilder quartz)
        where TJobConfiguration : IJobConfiguration, new()
    {
        new TJobConfiguration().Schedule(quartz);
    }
}

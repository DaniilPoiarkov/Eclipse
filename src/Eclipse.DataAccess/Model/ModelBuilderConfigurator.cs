using Eclipse.DataAccess.CosmosDb;
using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.MoodRecords;
using Eclipse.Domain.OutboxMessages;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eclipse.DataAccess.Model;

internal sealed class ModelBuilderConfigurator : IModelBuilderConfigurator
{
    private readonly IOptions<CosmosDbContextOptions> _options;

    private readonly IEntityTypeConfiguration<User> _userConfiguration;

    private readonly IEntityTypeConfiguration<OutboxMessage> _outboxMessageConfiguration;

    private readonly IEntityTypeConfiguration<MoodRecord> _moodRecordConfiguration;

    private readonly IEntityTypeConfiguration<UserStatistics> _userStatisticsConfiguration;

    private readonly IEntityTypeConfiguration<InboxMessage> _inboxMessageConfiguration;

    public ModelBuilderConfigurator(
        IOptions<CosmosDbContextOptions> options,
        IEntityTypeConfiguration<User> userConfiguration,
        IEntityTypeConfiguration<OutboxMessage> outboundMessageConfiguration,
        IEntityTypeConfiguration<MoodRecord> moodRecordConfiguration,
        IEntityTypeConfiguration<UserStatistics> userStatisticsConfiguration,
        IEntityTypeConfiguration<InboxMessage> inboxMessageConfiguration)
    {
        _options = options;

        _userConfiguration = userConfiguration;
        _outboxMessageConfiguration = outboundMessageConfiguration;
        _moodRecordConfiguration = moodRecordConfiguration;
        _userStatisticsConfiguration = userStatisticsConfiguration;
        _inboxMessageConfiguration = inboxMessageConfiguration;
    }

    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(_userConfiguration);
        modelBuilder.ApplyConfiguration(_outboxMessageConfiguration);
        modelBuilder.ApplyConfiguration(_moodRecordConfiguration);
        modelBuilder.ApplyConfiguration(_userStatisticsConfiguration);
        modelBuilder.ApplyConfiguration(_inboxMessageConfiguration);

        modelBuilder.HasDefaultContainer(_options.Value.Container);
    }
}
